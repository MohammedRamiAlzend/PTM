using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace PTM.Application.Commands
{
    public record RemoveTaskCommand(int TaskId):IRequest<ApiResponse>;
    public class RemoveTaskCommandHandle(IEntityCommiter commiter, ILogger<RemoveTaskCommandHandle> logger) : IRequestHandler<RemoveTaskCommand, ApiResponse>
    {
        public async Task<ApiResponse> Handle(RemoveTaskCommand request, CancellationToken cancellationToken)
        {
            var removeResult = await commiter.Tasks.RemoveAsync(x=>x.Id == request.TaskId);
            if(removeResult.IsSuccess is false)
            {
                logger.LogError("Failed to remove task with Id: {TaskId}. Error: {ErrorMessage}", request.TaskId, removeResult.Message);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, removeResult.Message!);            }
            var commitResult = await commiter.CommitAsync(cancellationToken);
            if(commitResult <= 0)
            {
                logger.LogError("Failed to commit changes after removing task with Id: {TaskId}.", request.TaskId);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Failed to save changes after removing the task.");
            }
            logger.LogInformation("Task with Id: {TaskId} removed successfully.", request.TaskId);
            return ApiResponse.Success();
        }
    }
}
