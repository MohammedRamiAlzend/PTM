using PTM.Application.DTOs.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Commands.CreateTask
{
    public record CreateTaskCommand() : IRequest<ApiResponse<TaskResponseDto>>;

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ApiResponse<TaskResponseDto>>
    {
        public async Task<ApiResponse<TaskResponseDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
