using FluentValidation;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System.Net;
namespace PTM.Application.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectDto Dto) : IRequest<ApiResponse<ProjectResponseDto>>;

public class CreateProjectCommandHandler(
    IEntityCommiter commiter,
    IMapper mapper,
    IValidator<CreateProjectDto> validator,
    ILogger<CreateProjectCommand> logger) : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    public async Task<ApiResponse<ProjectResponseDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var validateResult = validator.Validate(request.Dto);
        if(validateResult.IsValid is false)
        {
            return ApiResponse<ProjectResponseDto>.Failure(HttpStatusCode.NotAcceptable, [.. validateResult.Errors.Select(x => x.ErrorMessage)]);
        }
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
