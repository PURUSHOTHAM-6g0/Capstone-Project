using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.TaskManagements.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusCommand : IRequest<bool>
    {
        public long TaskId { get; set; }
        public int Status { get; set; }
    }
    public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, bool>
    {
        private readonly TaskDbcontext _context;
        public UpdateTaskStatusHandler(TaskDbcontext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var status= await _context.Tasks.FirstOrDefaultAsync(c => c.TaskId == request.TaskId, cancellationToken);

            if (status == null)
            {
                return false;
            }
            if (!Enum.IsDefined(typeof(Tasks.TaskStatus), request.Status))
            {
                throw new ArgumentException("Invalid status value.");
            }
            status.Status = (Tasks.TaskStatus)request.Status;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
