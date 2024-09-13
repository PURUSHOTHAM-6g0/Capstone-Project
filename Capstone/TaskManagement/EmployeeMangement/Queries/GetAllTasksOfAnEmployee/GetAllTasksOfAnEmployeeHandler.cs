using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Dto;

namespace TaskManagement.EmployeeMangement.Queries.GetAllTasksOfAnEmployee
{
    public class GetAllTasksOfAnEmployeeQuery : IRequest<List<GetAllTasksOfAnEmployeeResponse>>
    {

    }

    public class GetAllTasksOfAnEmployeeResponse
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public string AssignedTo {  get; set; }
        public DateTime DueDate { get; set; }
    }

    public class GetAllTasksOfAnEmployeeHandler : IRequestHandler<GetAllTasksOfAnEmployeeQuery, List<GetAllTasksOfAnEmployeeResponse>>
    {
        private readonly TaskDbcontext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllTasksOfAnEmployeeHandler(IHttpContextAccessor httpContextAccessor, TaskDbcontext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetAllTasksOfAnEmployeeResponse>> Handle(GetAllTasksOfAnEmployeeQuery request, CancellationToken cancellationToken)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            var employeeId = claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(employeeId))
            {
                return new List<GetAllTasksOfAnEmployeeResponse>();
            }

            // First query: Fetch tasks directly assigned to the employee
            var directlyAssignedTasks = await _context.Tasks
                .Where(t => t.AssignedTo == employeeId)
                .Select(t => new GetAllTasksOfAnEmployeeResponse
                {
                    TaskId = t.TaskId,
                    TaskName = t.TaskName,
                    TaskDescription = t.TaskDescription,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate,
                    AssignedTo=t.AssignedTo
                })
                .ToListAsync(cancellationToken);

            // Second query: Fetch tasks from projects the employee is working on
            var projectRelatedTasks = await _context.EmployeeProjects
                .Where(ep => ep.EmployeeId == employeeId)
                .SelectMany(ep => _context.Tasks
                    .Where(t => t.ProjectId == ep.ProjectId)
                    .Select(t => new GetAllTasksOfAnEmployeeResponse
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskDescription = t.TaskDescription,
                        Status = t.Status.ToString(),
                        DueDate = t.DueDate,
                        AssignedTo= t.AssignedTo
                    }))
                .ToListAsync(cancellationToken);

            // Combine the two sets of tasks
            var allTasks = directlyAssignedTasks.Concat(projectRelatedTasks).ToList();

            return allTasks;
        }
    }
}
