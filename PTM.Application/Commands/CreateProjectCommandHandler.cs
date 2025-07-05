using FluentValidation;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System.Net;
using Microsoft.Extensions.Logging;
namespace PTM.Application.Commands;

public record CreateProjectCommand(CreateProjectDto Dto) : IRequest<ApiResponse<ProjectResponseDto>>;

public class CreateProjectCommandHandler(
    IEntityCommiter commiter,
    IMapper mapper,
    IValidator<CreateProjectDto> validator,
    ILogger<CreateProjectCommand> logger) : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    public async Task<ApiResponse<ProjectResponseDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to create a new project with Name: {ProjectName}", request.Dto.Name);
        var validateResult = validator.Validate(request.Dto);
        if(validateResult.IsValid is false)
        {
            logger.LogWarning("Project creation validation failed for Name: {ProjectName}. Errors: {@Errors}", request.Dto.Name, validateResult.Errors.Select(x => x.ErrorMessage));
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
            logger.LogError("Failed to add new project {ProjectName} to the database. Error: {ErrorMessage}", request.Dto.Name, addResult.Message);
            return ApiResponse<ProjectResponseDto>.Failure(HttpStatusCode.InternalServerError, addResult.Message!);
        }
        
        await commiter.CommitAsync(cancellationToken);
        
        var mapToResponse = mapper.Map<ProjectResponseDto>(project);
        logger.LogInformation("Project {ProjectName} created successfully with Id: {ProjectId}", project.Name, project.Id);
        return ApiResponse<ProjectResponseDto>.Success(mapToResponse);

    }
}
