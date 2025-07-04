using PTM.Domain.Entities;

namespace PTM.Application.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectDto Dto) : IRequest<ApiResponse<ProjectResponseDto>>;

public class CreateProjectCommandHandler(
    IEntityCommiter commiter,
    IMapper mapper,
    ILogger<CreateProjectCommand> logger) : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    public async Task<ApiResponse<ProjectResponseDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project()
        {
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Tasks = []
        };

        var addResult = await commiter.Projects.AddAsync(project);
        if (addResult.IsSuccess is false)
        {
            return ApiResponse<ProjectResponseDto>.Failure(System.Net.HttpStatusCode.InternalServerError, addResult.Message!);
        }
        
        await commiter.CommitAsync(cancellationToken);
        
        var mapToResponse = mapper.Map<ProjectResponseDto>(project);
        return ApiResponse<ProjectResponseDto>.Success(mapToResponse);

    }
}
