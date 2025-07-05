using FluentValidation;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System.Net;
using Microsoft.Extensions.Logging;
namespace PTM.Application.Commands
{
    public record CreateTaskCommand(int ProjectId, CreateTaskDto Dto) : IRequest<ApiResponse<TaskResponseDto>>;



    public class CreateTaskCommandHandler(
        IEntityCommiter commiter,
        IMapper mapper,
        IValidator<CreateTaskDto> validator,
        ILogger<CreateTaskCommand> logger) : IRequestHandler<CreateTaskCommand, ApiResponse<TaskResponseDto>>
    {
        public async Task<ApiResponse<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to create a new task for ProjectId: {ProjectId} with Title: {TaskTitle}", request.ProjectId, request.Dto.Title);
            var validResult = validator.Validate(request.Dto);
            if (validResult.IsValid is false)
            {
                logger.LogWarning("Task creation validation failed for ProjectId: {ProjectId}, Title: {TaskTitle}. Errors: {@Errors}", request.ProjectId, request.Dto.Title, validResult.Errors.Select(x => x.ErrorMessage));
                return ApiResponse<TaskResponseDto>.Failure(HttpStatusCode.NotAcceptable,
                    [.. validResult.Errors.Select(x => x.ErrorMessage)]);
            }
            if(!await commiter.Projects.AnyAsync(x=>x.Id == request.ProjectId))
            {
                logger.LogWarning("Project with ID {ProjectId} was not found for task creation.", request.ProjectId);
                return ApiResponse<TaskResponseDto>.Failure(HttpStatusCode.NotFound, $"project with {request.ProjectId} was not found");
            }
            var task = new AppTask()
            {
                ProjectId = request.ProjectId,
                DueDate = request.Dto.DueDate,
                Title = request.Dto.Title,
            };
            var addingResult = await commiter.Tasks.AddAsync(task);
            if (addingResult.IsSuccess is false)
            {
                logger.LogError("Failed to add new task {TaskTitle} to project {ProjectId}. Error: {ErrorMessage}", request.Dto.Title, request.ProjectId, addingResult.Message);
                return ApiResponse<TaskResponseDto>.Failure(HttpStatusCode.InternalServerError, addingResult.Message!);
            }
            await commiter.CommitAsync(cancellationToken);
            var mapToDto = mapper.Map<TaskResponseDto>(task);
            logger.LogInformation("Task {TaskTitle} created successfully with Id: {TaskId} for ProjectId: {ProjectId}", task.Title, task.Id, task.ProjectId);
            return ApiResponse<TaskResponseDto>.Success(mapToDto);
        }
    }
}
