using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Domain.Aggregates.Members.Entities;

public sealed class Membership
{
    private Membership(string id, MembershipType membershipType, DateTime startDateUtc, DateTime? endDateUtc, decimal monthlyPrice)
    {
        Id = GuidValidator.EnsureValidGuid(id);

        if (membershipType is null)
            throw new ValidationDomainException("Membership Type is required.");
        MembershipType = membershipType;
        
        if (startDateUtc < DateTime.UtcNow)        
            throw new ValidationDomainException("Start date cannot be before current time.");
        if (startDateUtc > endDateUtc)
            throw new ValidationDomainException("Start date cannot be after end date.");

        StartDateUtc = startDateUtc;        
        EndDateUtc = endDateUtc;
   
        MonthlyPrice = PriceValidation.IsNotNegative(monthlyPrice, nameof(monthlyPrice));
    }

    public string Id { get; private set; } = null!;
    public MembershipType MembershipType { get; private set; }
    public DateTime StartDateUtc { get; private set; }
    public DateTime? EndDateUtc { get; private set; } = null!;
    public decimal MonthlyPrice { get; private set; }
    public bool IsActive => !EndDateUtc.HasValue || DateTime.UtcNow <= EndDateUtc;

    public static Membership Create(MembershipType membershipType, DateTime startdate, decimal monthlyPrice) 
        => new(Guid.NewGuid().ToString(), membershipType, startdate, null, monthlyPrice);

    public static Membership Rehydrate(string id, MembershipType membershipType, DateTime startdate, DateTime? endtdate, decimal monthlyPrice) 
        => new(id, membershipType, startdate, endtdate, monthlyPrice);

    internal void Deactivate()
    {
        if (!IsActive)         
            throw new ValidationDomainException("Can't deactivate an already deactivated membership");

        EndDateUtc = DateTime.UtcNow;
    }
}
