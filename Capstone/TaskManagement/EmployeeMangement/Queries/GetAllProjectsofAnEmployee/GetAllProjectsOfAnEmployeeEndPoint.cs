using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;

namespace TaskManagement.EmployeeMangement.Queries.GetAllProjectsofAnEmployee
{
    public class GetAllProjectsOfAnEmployeeEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/employee/projects/tasks", async(ISender sender) =>
            {
                var query = new GetAllProjectsOfAnEmployeeQuery();
                var result= await sender.Send(query);
                if (result == null || result.Count == 0)
                {
                    return Results.NotFound(new { Error = "No projects found for the employee." });
                }
                return Results.Ok(result);
            });
        }
    }
}
