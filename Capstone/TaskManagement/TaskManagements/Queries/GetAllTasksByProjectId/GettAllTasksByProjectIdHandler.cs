using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Dto;

namespace TaskManagement.TaskManagements.Queries.GetAllTasksByProjectId
{
    public class GetallTasksByProjectIdQuery : IRequest<GetAllTasksByProjectIdResponse>
    {
        public int ProjectId { get; set; }
    }
    public class GetAllTasksByProjectIdResponse
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
    public class GettAllTasksByProjectIdHandler : IRequestHandler<GetallTasksByProjectIdQuery, GetAllTasksByProjectIdResponse>
    {
        private readonly TaskDbcontext _context;

        public GettAllTasksByProjectIdHandler(TaskDbcontext context)
        {
            _context = context;
        }
        public async Task<GetAllTasksByProjectIdResponse> Handle(GetallTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                 .Include(p => p.Tasks)
                 .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId, cancellationToken);

            if (project == null)
            {
                return null; 
            }
            
            var response = new GetAllTasksByProjectIdResponse
            {
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                Tasks = project.Tasks.Select(t => new TaskDto
                {
                    TaskId = t.TaskId,
                    TaskName = t.TaskName,
                    TaskDescription = t.TaskDescription,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate
                }).ToList()
            };

            return response;
        }
    }
}
