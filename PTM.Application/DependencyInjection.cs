using PTM.Application.Queries.GetAllProjects;
using PTM.Application.Queries.GetAllTasks;

namespace PTM.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddApplicationDI(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(CreateTaskCommand).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(GetAllProjectsQuery).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(GetAllTasksQuery).Assembly);
            
            });
            
            
        }
    }
}
