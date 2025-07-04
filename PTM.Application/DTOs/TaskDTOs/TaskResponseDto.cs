using PTM.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.DTOs.TaskDTOs
{
    public class TaskResponseDto
    {
        public required int Id { get; set; }
        public string? Title { get; set; }
        public required AppTaskStatus Status { get; set; }
        public required DateTime DueDate { get; set; }
    }
}
