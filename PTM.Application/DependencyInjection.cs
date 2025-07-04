using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PTM.Application;
using PTM.Application.Commands;
using PTM.Application.Queries;
using PTM.Application.Validation;

namespace PTM.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddApplicationDI(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(GetProjectsQuery).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly);
            
            });

            services.AddValidatorsFromAssembly(typeof(CreateTaskValidation).Assembly);


            services.AddAutoMapper(typeof(Helper));
        }
    }
}
