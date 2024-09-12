using MediatR;
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
    }
    public class AddTaskForProjectResponse
    {
        public int TaskId { get; set; }
    }
    public class AddTaskForProjectHandler : IRequestHandler<AddTaskForProjectCommand, AddTaskForProjectResponse>
    {
        private readonly TaskDbcontext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public AddTaskForProjectHandler(TaskDbcontext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpContextAccessor;
        }
        public async Task<AddTaskForProjectResponse> Handle(AddTaskForProjectCommand request, CancellationToken cancellationToken)
        {
            var claimsPrincipal = _httpcontextAccessor.HttpContext.User;
            var employeeTime= claimsPrincipal.FindFirst(ClaimTypes.Country)?.Value;
            var employeeId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(employeeId))
            {
                throw new UnauthorizedAccessException("Employee ID is missing in the token");
            }

            var project = await _context.Projects
                .FindAsync(request.ProjectId);

            if (project == null)
            {
                throw new ArgumentException("Project not found");
            }
            DateTime taskDueDateInUtc;
            try
            {
                var employeeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(employeeTime);
                taskDueDateInUtc = TimeZoneInfo.ConvertTimeToUtc(request.DueDate, employeeTimeZone);
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
                EmployeeId=employeeId,
                AssignedTo=employeeId
                
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddTaskForProjectResponse { TaskId = task.TaskId };
        }
    }
}
