using Microsoft.EntityFrameworkCore;
using System.Net;

namespace PTM.Application.Queries
{
    public record GetProjectsQuery() : IRequest<ApiResponse<List<ProjectResponseDto>>>;
    public class GetProjectsQueryHandler(
        IEntityCommiter commiter,
        ILogger<GetProjectsQuery> logger,
        IMapper mapper) : IRequestHandler<GetProjectsQuery, ApiResponse<List<ProjectResponseDto>>>
    {
        public async Task<ApiResponse<List<ProjectResponseDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to retrieve all projects.");
            var getProjectsRequest = await commiter.Projects.GetAllAsync(include: i => i.Include(x => x.Tasks));
            if (getProjectsRequest.IsSuccess is false)
            {
                logger.LogError("Failed to retrieve projects: {ErrorMessage}", getProjectsRequest.Message);
                return ApiResponse < List<ProjectResponseDto>>.Failure(HttpStatusCode.InternalServerError,getProjectsRequest.Message!);
            }
            var data = getProjectsRequest.Data;
            var projectsAsDtos = mapper.Map<List<ProjectResponseDto>>(data);
            logger.LogInformation("Successfully retrieved {Count} projects.", projectsAsDtos.Count);
            return ApiResponse<List<ProjectResponseDto>>.Success(projectsAsDtos);
        }
    }
}
