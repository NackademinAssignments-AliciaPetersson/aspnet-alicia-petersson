using Application.Abstractions.Persistence;
using Infrastructure.Extensions.Logging;
using Infrastructure.Extensions.Persistence;
using Infrastructure.Persistence.EFC.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceCollectionRegistrationExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddCustomLogging();
        services.AddPersistence(configuration, environment);
        services.AddScoped<IUnitOfWork, UnitOfWork>();        

        return services;
    }    
}