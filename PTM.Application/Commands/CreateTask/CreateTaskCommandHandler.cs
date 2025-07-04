using PTM.Application.DTOs.TaskDTOs;
namespace PTM.Application.Commands.CreateTask
{
    public record CreateTaskCommand(int projectId, CreateTaskDto Requets) : IRequest<ApiResponse<TaskResponseDto>>;



    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ApiResponse<TaskResponseDto>>
    {
        public async Task<ApiResponse<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
