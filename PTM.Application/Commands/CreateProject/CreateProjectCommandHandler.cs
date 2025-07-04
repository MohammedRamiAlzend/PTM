namespace PTM.Application.Commands.CreateProject;

public record CreateProjectCommand(string Name, string Description) : IRequest<ApiResponse<ProjectResponseDto>>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    public async Task<ApiResponse<ProjectResponseDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
