using PTM.Domain.Entities.Enums;
using PTM.Domain.Entities.Interfaces;

namespace PTM.Domain.Entities;

public class AppTask : IEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public AppTaskStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}
