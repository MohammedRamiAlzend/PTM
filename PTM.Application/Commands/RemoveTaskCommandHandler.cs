using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Commands
{
    public record RemoveTaskCommand(int TaskId):IRequest<ApiResponse>;
    public class RemoveTaskCommandHandle(IEntityCommiter commiter) : IRequestHandler<RemoveTaskCommand, ApiResponse>
    {
        public async Task<ApiResponse> Handle(RemoveTaskCommand request, CancellationToken cancellationToken)
        {
            await commiter.Tasks.RemoveAsync(x=>x.Id == request.TaskId);
            await commiter.CommitAsync();
            return ApiResponse.Success();
        }
    }
}
