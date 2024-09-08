using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.ProjectsMangement.Commands.AddProject
{
    public class AddProjectCommand : IRequest<AddProjectResponse>
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
    }

    public class AddProjectResponse
    {
        public int ProjectId { get; set; }
    }

    public class AddProjectHandler : IRequestHandler<AddProjectCommand, AddProjectResponse>
    {
        private readonly TaskDbcontext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public AddProjectHandler(TaskDbcontext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<AddProjectResponse> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            var claimsPrincipal = _contextAccessor.HttpContext.User;
            var employeeId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employeeName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            var employeeTimeZone = claimsPrincipal.FindFirst(ClaimTypes.Country)?.Value; // Adjust claim type if needed

            if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(employeeName) || string.IsNullOrEmpty(employeeTimeZone))
            {
                throw new UnauthorizedAccessException("Employee ID, Name, or TimeZone is missing in the token");
            }

            // Ensure Employee exists or add it
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                employee = new Employee
                {
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    TimeZone = employeeTimeZone,
                };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Check if the project already exists
            var project = await _context.Projects
    .FirstOrDefaultAsync(p => p.ProjectName == request.ProjectName, cancellationToken);

            if (project == null)
            {
                project = new Project
                {
                    ProjectDescription = request.ProjectDescription,
                    ProjectName = request.ProjectName,
                    EmployeeId = employeeId // Set EmployeeId if necessary
                };
                _context.Projects.Add(project);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var employeeProject = await _context.EmployeeProjects
                .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == project.ProjectId, cancellationToken);

            if (employeeProject == null)
            {
                employeeProject = new EmployeeProject
                {
                    EmployeeId = employeeId,
                    ProjectId = project.ProjectId,
                };
                _context.EmployeeProjects.Add(employeeProject);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new AddProjectResponse { ProjectId = project.ProjectId };
        }
    }
}
