using Microsoft.Extensions.DependencyInjection;
using PTM.Application.Commands.CreateProject;

namespace PTM.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddApplicationDI(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
            });
            
        }
    }
}
