using Microsoft.EntityFrameworkCore;
using PTM.Application.DTOs.TaskDTOs;
using System.Net;

namespace PTM.Application.Queries;

public record GetTasksQuery(int ProjectId) : IRequest<ApiResponse<List<TaskResponseDto>>>;
public class GetTasksQueryHandler(
    IEntityCommiter commiter,
    IMapper mapper,
    ILogger<GetTasksQuery> logger) : IRequestHandler<GetTasksQuery, ApiResponse<List<TaskResponseDto>>>
{
    public async Task<ApiResponse<List<TaskResponseDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to retrieve all tasks for project with ID: {ProjectId}", request.ProjectId);
        var getTasksRequest = await commiter.Tasks.GetAllAsync(
            include: i => i.Include(x => x.Project), 
            filter: x => x.ProjectId == request.ProjectId);

        if (getTasksRequest.IsSuccess is false) 
        {
            logger.LogError("Failed to retrieve tasks for project {ProjectId}: {ErrorMessage}", request.ProjectId, getTasksRequest.Message);
            return ApiResponse<List<TaskResponseDto>>.Failure(HttpStatusCode.InternalServerError,getTasksRequest.Message!);
        }

        var data = getTasksRequest.Data;
        var tasksAsDtos = mapper.Map<List<TaskResponseDto>>(data);
        logger.LogInformation("Successfully retrieved {Count} tasks for project with ID: {ProjectId}", tasksAsDtos.Count, request.ProjectId);
        return ApiResponse<List<TaskResponseDto>>.Success(tasksAsDtos, messages: "Data retrieved successfully");
    }
}
