
namespace PTM.Infrastructure.Data;
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppTask> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
}
