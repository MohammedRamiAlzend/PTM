using PTM.Application.DTOs.TaskDTOs;

namespace PTM.Application.DTOs.ProjectDTOs
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public  string Name { get; set; }
        public  string Description { get; set; }
        public  List<TaskResponseDto> Tasks { get; set; } = [];
    }
}
