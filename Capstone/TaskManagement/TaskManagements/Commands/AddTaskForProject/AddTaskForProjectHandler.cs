using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.TaskManagements.Commands.AddTaskForProject
{
    public class AddTaskForProjectCommand : IRequest<AddTaskForProjectResponse>
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public int ProjectId { get; set; }
        public string AssignedTo {  get; set; }
    }
    public class AddTaskForProjectResponse
    {
        public int TaskId { get; set; }
    }
    public class AddTaskForProjectHandler : IRequestHandler<AddTaskForProjectCommand, AddTaskForProjectResponse>
    {
        private readonly TaskDbcontext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public AddTaskForProjectHandler(TaskDbcontext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpContextAccessor;
        }

        public async Task<AddTaskForProjectResponse> Handle(AddTaskForProjectCommand request, CancellationToken cancellationToken)
        {
           
            var claimsPrincipal = _httpcontextAccessor.HttpContext.User;
            var employeeTimeZone = claimsPrincipal.FindFirst(ClaimTypes.Country)?.Value;
            var employeeId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Creator's ID

            if (string.IsNullOrEmpty(employeeId))
            {
                throw new UnauthorizedAccessException("Employee ID is missing in the token");
            }

           
            var project = await _context.Projects.FindAsync(request.ProjectId);
            if (project == null)
            {
                throw new ArgumentException("Project not found");
            }

          
            var assignedEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeName == request.AssignedTo);

            if (assignedEmployee == null)
            {
                throw new ArgumentException("Assigned employee not found");
            }

            
            DateTime taskDueDateInUtc;
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(employeeTimeZone);
                taskDueDateInUtc = TimeZoneInfo.ConvertTimeToUtc(request.DueDate, timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new ArgumentException("Invalid time zone provided.");
            }

            
            var task = new Tasks
            {
                TaskName = request.TaskName,
                TaskDescription = request.TaskDescription,
                ProjectId = request.ProjectId,
                DueDate = taskDueDateInUtc,
                Status = Tasks.TaskStatus.Todo,
                EmployeeId = employeeId,  
                AssignedTo = assignedEmployee.EmployeeName 
            };

           
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            
            var employeeProject = await _context.EmployeeProjects
                .FirstOrDefaultAsync(ep => ep.EmployeeId == assignedEmployee.EmployeeId && ep.ProjectId == request.ProjectId);

            if (employeeProject != null)
            {
                employeeProject.TaskId = task.TaskId;
                _context.EmployeeProjects.Update(employeeProject);
                await _context.SaveChangesAsync(cancellationToken); 
            }
            else
            {
                
                var newEmployeeProject = new EmployeeProject
                {
                    EmployeeId = assignedEmployee.EmployeeId,
                    ProjectId = request.ProjectId,
                    TaskId = task.TaskId
                };
                _context.EmployeeProjects.Add(newEmployeeProject);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new AddTaskForProjectResponse { TaskId = task.TaskId };
        }
    }


}
