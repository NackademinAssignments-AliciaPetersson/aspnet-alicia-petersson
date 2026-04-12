using Application.Abstractions.Persistence;
using Infrastructure.Persistence.EFC.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.Persistence;

public static class RepositoriesRegistrationExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IContactRequestRepository, ContactRequestRepository>();
        services.AddScoped<IMembershipTypeRepository, MembershipTypeRepository>();

        return services;
    }
}
