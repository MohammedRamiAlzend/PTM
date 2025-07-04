using PTM.Domain.Entities.Enums;

namespace PTM.Domain.Entities;

public class AppTask : Entity
{
    public string? Title { get; set; }
    public AppTaskStatus Status { get; set; } = AppTaskStatus.Pending;
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}
