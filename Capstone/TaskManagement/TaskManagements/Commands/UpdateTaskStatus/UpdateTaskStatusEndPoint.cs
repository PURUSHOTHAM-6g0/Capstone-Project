using Carter;
using MediatR;

namespace TaskManagement.TaskManagements.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/task/{Taskid}/status", async(UpdateTaskStatusCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result ? Results.Ok(new { Message = "Task status updated successfully" }) : Results.StatusCode(500);
            });
        }
    }
}
