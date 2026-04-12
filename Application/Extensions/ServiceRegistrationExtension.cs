using Application.Abstractions.Services;
using Application.Modules.ContactRequests;
using Application.Modules.Members;
using Application.Modules.MembershipTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IContactRequestService, ContactRequestService>();
        services.AddScoped<IMembershipTypeService, MembershipTypeService>();

        return services;
    }
}