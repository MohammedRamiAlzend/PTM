using PTM.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.DTOs.TaskDTOs
{
    public class CreateTaskDto
    {
        public string? Title { get; set; }
        public AppTaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}
