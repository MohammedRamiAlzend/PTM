using PTM.Domain.Entities.Enums;
using PTM.Domain.Entities.Interfaces;

namespace PTM.Domain.Entities;

public class AppTask : Entity, ISoftDeletable
{
    public string? Title { get; set; }
    public AppTaskStatus Status { get; set; } = AppTaskStatus.Pending;
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public bool IsDeleted { get; set; }
}
