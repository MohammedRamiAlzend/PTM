using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
namespace PTM.Application.Commands.CreateTask
{
    public record CreateTaskCommand(int ProjectId, CreateTaskDto Dto) : IRequest<ApiResponse<TaskResponseDto>>;



    public class CreateTaskCommandHandler(
        IEntityCommiter commiter,
        IMapper mapper,
        ILogger<CreateTaskCommand> logger) : IRequestHandler<CreateTaskCommand, ApiResponse<TaskResponseDto>>
    {
        public async Task<ApiResponse<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = new AppTask()
            {
                ProjectId = request.ProjectId,
                DueDate = request.Dto.DueDate,
                Status = request.Dto.Status,
                Title = request.Dto.Title,
            };
            var addingResult = await commiter.Tasks.AddAsync(task);
            if(addingResult.IsSuccess is false)
            {
                return ApiResponse<TaskResponseDto>.Failure(System.Net.HttpStatusCode.InternalServerError, addingResult.Message!);
            }
            await commiter.CommitAsync(cancellationToken);
            var mapToDto= mapper.Map<TaskResponseDto>(task);
            return ApiResponse<TaskResponseDto>.Success(mapToDto);
        }
    }
}
