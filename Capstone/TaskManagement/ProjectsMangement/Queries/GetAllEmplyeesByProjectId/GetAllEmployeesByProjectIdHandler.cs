using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Dto;

namespace TaskManagement.ProjectsMangement.Queries.GetAllEmplyeesByProjectId
{
    public class GetAllEmployeesWithProjectIdQuery : IRequest<List<GetAllEmployeeWithProjectIdDto>>
    {
        public int ProjectId { get; set; }
    }
    public class GetAllEmployeesByProjectIdHandler : IRequestHandler<GetAllEmployeesWithProjectIdQuery, List<GetAllEmployeeWithProjectIdDto>>
    {
        private readonly TaskDbcontext _context;
        public GetAllEmployeesByProjectIdHandler(TaskDbcontext context)
        {
            _context = context;
        }
        public async Task<List<GetAllEmployeeWithProjectIdDto>> Handle(GetAllEmployeesWithProjectIdQuery request, CancellationToken cancellationToken)
        {
            var employees = await(from ep in _context.EmployeeProjects
                                  join e in _context.Employees on ep.EmployeeId equals e.EmployeeId
                                  where ep.ProjectId == request.ProjectId
                                  select new GetAllEmployeeWithProjectIdDto
                                  {
                                      EmployeeName = e.EmployeeName,
                                      TimeZone = e.TimeZone
                                  }).ToListAsync(cancellationToken);

            return employees;
        }
    }
}
