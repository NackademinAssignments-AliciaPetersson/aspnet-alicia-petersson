using Application.Abstractions.Services;
using Application.Modules.Members;
using Domain.Aggregates.Member;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMemberService, MemberService>();

        return services;
    }
}
