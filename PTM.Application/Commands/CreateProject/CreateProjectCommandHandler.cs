namespace PTM.Application.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectDto Request) : IRequest<ApiResponse<ProjectResponseDto>>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    public async Task<ApiResponse<ProjectResponseDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
