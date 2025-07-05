using Microsoft.Extensions.Logging;
using System.Net;

namespace PTM.Application.Commands;

public record RemoveProjectCommand(int ProjectId):IRequest<ApiResponse>;

public class RemoveProjectCommandHandler(IEntityCommiter commiter, ILogger<RemoveProjectCommandHandler> logger) : IRequestHandler<RemoveProjectCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to remove project with Id: {ProjectId}", request.ProjectId);
        var removeResult = await commiter.Projects.RemoveAsync(x => x.Id == request.ProjectId);
        if (removeResult.IsSuccess is false)
        {
            logger.LogError("Failed to remove project with Id: {ProjectId}. Error: {ErrorMessage}", request.ProjectId, removeResult.Message);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, removeResult.Message!);
        }

        var commitResult = await commiter.CommitAsync(cancellationToken);
        if (commitResult <= 0)
        {
            logger.LogError("Failed to commit changes after removing project with Id: {ProjectId}.", request.ProjectId);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Failed to save changes after removing the project.");
        }

        logger.LogInformation("Project with Id: {ProjectId} removed successfully.", request.ProjectId);
        return ApiResponse.Success(HttpStatusCode.OK, "Project removed successfully.");
    }
} 