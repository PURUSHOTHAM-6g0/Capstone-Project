using Carter;
using MediatR;

namespace TaskManagement.TaskManagements.Commands.AddTaskForProject
{
    public class AddTaskForProjectEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/task", async (AddTaskForProjectCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                Results.Created($"/task/{result.TaskId}", result);
            });
        }
    }
}
