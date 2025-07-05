using Microsoft.AspNet.Identity;
namespace PTM.Infrastructure.Data;
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppTask> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "AIRjPkYQbFLS9DWuVCJ+AVsX4trJx9jey5/1GOpd80bcQvZieH968kt2mGtZAGjBmA==",
                RoleId = 1
            },
            new User
            {
                Id = 2,
                Username = "user",
                PasswordHash = "AIRjPkYQbFLS9DWuVCJ+AVsX4trJx9jey5/1GOpd80bcQvZieH968kt2mGtZAGjBmA==",
                RoleId = 2
            }
        );


        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<AppTask>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}
