using Application.Abstractions.Services;
using Application.Modules.MembershipTypes.Inputs;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.EFC.Contexts;

internal static class ContextInitilizer
{
    public static async Task InitilizeDefaultMembershipTypes(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var membershipTypeService = scope.ServiceProvider.GetRequiredService<IMembershipTypeService>();      

        try
        {
            var existingTypesResult = await membershipTypeService.GetMembershipTypesAsync();
            if (!existingTypesResult.Success || existingTypesResult.Value is null || existingTypesResult.Value.Any())
                return;            

            var membershipTypeInputs = new List<CreateMembershipTypeInput>()
            {
                new("Standard Membership", 495),
                new ("Premium Membership", 595),
            };

            foreach (CreateMembershipTypeInput typeInput in membershipTypeInputs)
            {
                await membershipTypeService.CreateMembershipTypeAsync(typeInput);
            }            
        }
        catch { }
    }

}
