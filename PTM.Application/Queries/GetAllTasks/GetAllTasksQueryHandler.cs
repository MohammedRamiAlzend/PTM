using PTM.Application.DTOs.TaskDTOs;

namespace PTM.Application.Queries.GetAllTasks;

public record GetAllTasksQuery(int projectId): IRequest<ApiResponse<List<TaskResponseDto>>>;
public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, ApiResponse<List<TaskResponseDto>>>
{
    public Task<ApiResponse<List<TaskResponseDto>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
