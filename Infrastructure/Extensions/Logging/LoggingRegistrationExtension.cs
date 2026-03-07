using Domain.Abstractions.Logging;
using Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.Logging;

public static class LoggingRegistrationExtension
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ILogger, Logger>();

        return services;
    }
}