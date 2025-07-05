using FluentValidation;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System.Net;
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
            var validResult = validator.Validate(request.Dto);
            if (validResult.IsValid is false)
            {
                return ApiResponse<TaskResponseDto>.Failure(HttpStatusCode.NotAcceptable,
                    [.. validResult.Errors.Select(x => x.ErrorMessage)]);
            }
            if(!await commiter.Projects.AnyAsync(x=>x.Id == request.ProjectId))
            {
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
                return ApiResponse<TaskResponseDto>.Failure(HttpStatusCode.InternalServerError, addingResult.Message!);
            }
            await commiter.CommitAsync(cancellationToken);
            var mapToDto = mapper.Map<TaskResponseDto>(task);
            return ApiResponse<TaskResponseDto>.Success(mapToDto);
        }
    }
}
