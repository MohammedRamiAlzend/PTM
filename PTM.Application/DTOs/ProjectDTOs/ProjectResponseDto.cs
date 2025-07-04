using PTM.Application.DTOs.TaskDTOs;

namespace PTM.Application.DTOs.ProjectDTOs
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<TaskResponseDto> Tasks { get; set; } = [];
    }
}
