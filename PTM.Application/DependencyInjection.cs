using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PTM.Application;
using PTM.Application.Queries.GetAllProjects;
using PTM.Application.Queries.GetAllTasks;
using PTM.Application.Validation;

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
                cfg.RegisterServicesFromAssemblies(typeof(GetTasksQuery).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(GetProjectsQuery).Assembly);
            
            });

            services.AddValidatorsFromAssembly(typeof(CreateTaskValidation).Assembly);


            services.AddAutoMapper(typeof(Helper));
        }
    }
}
