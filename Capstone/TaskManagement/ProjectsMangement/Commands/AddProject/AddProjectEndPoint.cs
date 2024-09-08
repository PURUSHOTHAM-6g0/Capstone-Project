using Carter;
using Mapster;
using MediatR;

namespace TaskManagement.ProjectsMangement.Commands.AddProject
{
    public class AddProjectRequest
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
    }
    public class AddProjectEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/project", async (AddProjectRequest request, ISender sender) =>
            {
                var command = request.Adapt<AddProjectCommand>();
                var result = await sender.Send(command);
                return Results.Created($"/project/{result.ProjectId}", result);
            });
        }
    }
}
