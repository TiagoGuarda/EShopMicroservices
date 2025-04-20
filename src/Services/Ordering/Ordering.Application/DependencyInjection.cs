using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ordering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //services.AddValidatorsFromAssemblyContaining<DependencyInjection>();
        return services;
    }
}
