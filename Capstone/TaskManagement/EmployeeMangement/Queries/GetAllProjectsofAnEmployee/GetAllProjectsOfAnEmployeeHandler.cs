using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TaskManagement.EmployeeMangement.Queries.GetAllProjectsofAnEmployee
{
    public class GetAllProjectsOfAnEmployeeQuery : IRequest<List<GetAllProjectsOfAnEmployeeRespone>> 
    {
        public string EmployeeId { get; set; }
    }

    public class GetAllProjectsOfAnEmployeeRespone
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }

    public class GetAllProjectsOfAnEmployeeHandler : IRequestHandler<GetAllProjectsOfAnEmployeeQuery, List<GetAllProjectsOfAnEmployeeRespone>>
    {
        private readonly TaskDbcontext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public GetAllProjectsOfAnEmployeeHandler(TaskDbcontext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpContextAccessor;
        }

        public async Task<List<GetAllProjectsOfAnEmployeeRespone>> Handle(GetAllProjectsOfAnEmployeeQuery request, CancellationToken cancellationToken)
        {
           
            var claimsPrincipal = _httpcontextAccessor.HttpContext.User;
            var employeeId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(employeeId))
            {
                
                return new List<GetAllProjectsOfAnEmployeeRespone>();
            }

            
            var projects = await _context.Projects
                .Where(p => p.EmployeeProjects.Any(ep => ep.EmployeeId == employeeId))
                .Select(p => new GetAllProjectsOfAnEmployeeRespone
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    Tasks = p.Tasks.Select(t => new TaskDto
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskDescription = t.TaskDescription,
                        Status = t.Status.ToString(),
                        DueDate = t.DueDate,
                        AssignedTo = t.AssignedTo,
                    }).ToList()
                })
                .ToListAsync(cancellationToken); 

            return projects; 
        }
    }
}
