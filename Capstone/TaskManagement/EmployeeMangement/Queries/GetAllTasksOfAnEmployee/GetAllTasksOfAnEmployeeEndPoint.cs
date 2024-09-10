using Carter;
using MediatR;

namespace TaskManagement.EmployeeMangement.Queries.GetAllTasksOfAnEmployee
{
    public class GetAllTasksOfAnEmployeeEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/employees/task", async (ISender sender) =>
            {
                var query = new GetAllTasksOfAnEmployeeQuery();
                var result = await sender.Send(query);
                if (result == null || result.Count == 0)
                {
                    return Results.NotFound(new { Error = "No Tasks found for the employee." });
                }
                return Results.Ok(result);
            });
        }
    }
}
