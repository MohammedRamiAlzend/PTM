using PTM.Infrastructure.Repositories;
using PTM.Infrastructure.Repositories.Interfaces;
using PTM.Infrastructure.BackgroundJobs;

namespace PTM.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDI(this IServiceCollection services,string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(option =>
            option.UseSqlite(connectionString)
        );

        services.AddScoped<IEntityCommiter, EntityCommiter>()
        .AddScoped(typeof(IPTMRepositoryBase<>), typeof(PTMRepositoryBase<>));

        services.AddHostedService<OverdueTaskBackgroundService>();
    }
}
