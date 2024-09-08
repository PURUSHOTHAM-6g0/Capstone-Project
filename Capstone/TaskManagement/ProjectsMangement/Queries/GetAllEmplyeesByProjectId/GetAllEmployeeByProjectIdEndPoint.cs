using Carter;
using MediatR;

namespace TaskManagement.ProjectsMangement.Queries.GetAllEmplyeesByProjectId
{
    public class GetAllEmployeesWithProjectIdEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/project/{projectId}/employees", async (int projectId, ISender sender) =>
            {
                var query = new GetAllEmployeesWithProjectIdQuery { ProjectId = projectId };
                var result = await sender.Send(query);
                return Results.Ok(result);
            });
        }
    }
}
