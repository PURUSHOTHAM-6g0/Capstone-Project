using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.ProjectsMangement.Commands.AddProject
{
    public class AddProjectCommand:IRequest<AddProjectResponse>
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
            
            var claimIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var employeeIdClaims = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var employeeNameClaims = claimIdentity?.FindFirst(ClaimTypes.Name);
            var employeeTimezone = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (employeeIdClaims == null || employeeNameClaims == null || employeeTimezone == null)
            {
                throw new UnauthorizedAccessException("Employee ID, Name, or TimeZone is missing in the token");
            }

            string employeeId = employeeIdClaims.Value;
            string employeeName = employeeNameClaims.Value;
            string employeeTimeZone = employeeTimezone.Value;

          
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                employee = new Models.Employee
                {
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                    TimeZone = employeeTimeZone,
                };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync(cancellationToken);
            }

           
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectName == request.ProjectName, cancellationToken);

            if (project == null)
            {
               
                project = new Project
                {
                    ProjectDescription = request.ProjectDescription,
                    ProjectName = request.ProjectName,
                };
                await _context.AddAsync(project, cancellationToken);
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
                await _context.EmployeeProjects.AddAsync(employeeProject, cancellationToken);
            }

            
            await _context.SaveChangesAsync(cancellationToken);

            return new AddProjectResponse { ProjectId = project.ProjectId };
        }
    }


}
