using Carter;
using MediatR;
using TaskManagement.Models;

namespace TaskManagement.TaskManagements.Queries.GetAllTasksByProjectId
{
    public class GetAllTasksByProjectIdEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/projects/tasks/{ProjectId}", async (int projectId, ISender sender) =>
            {
               
                var query = new GetallTasksByProjectIdQuery { ProjectId = projectId };
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.NotFound(new { Error = "Project not found." });
                }
                return Results.Ok(result);
            });
        }
    }
}
