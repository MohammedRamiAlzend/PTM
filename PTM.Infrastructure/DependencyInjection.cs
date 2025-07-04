namespace PTM.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDI(this IServiceCollection services,string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(option =>
            option.UseSqlite(connectionString)
        );
    }
}
