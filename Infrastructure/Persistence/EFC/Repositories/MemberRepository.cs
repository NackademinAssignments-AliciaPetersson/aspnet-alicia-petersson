using Application.Abstractions.Persistence;
using Domain.Abstractions.Logging;
using Domain.Aggregates.Members;
using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;
using Infrastructure.Persistence.EFC.Contexts;
using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EFC.Repositories;

public class MemberRepository(CoreFitnessContext context, ILogger logger) : RepositoryBase<Member, string, MemberEntity, CoreFitnessContext>(context, logger), IMemberRepository
{
    public async override Task<Member?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var entity = await Set
            .AsNoTracking()
            .Include(m => m.Memberships)
            .ThenInclude(m => m.MembershipType)
            .SingleOrDefaultAsync(m => m.Id == id, ct);

        return entity is not null ? ToDomainModel(entity) : null;
    }
    public async Task<Member?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var entity = await Set
                .AsNoTracking()
                .Include(m => m.Memberships)
                .ThenInclude(m => m.MembershipType)
                .SingleOrDefaultAsync(e => e.UserId == userId, ct);
            return entity is null ? default : ToDomainModel(entity);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Log(ex);
            throw;
        }
    }

    public async override Task<Member?> UpdateAsync(string id, Member model, CancellationToken ct = default)
    {
        try
        {
            if (model is null)
                throw new NullDomainException("Domain model must be provided.");

            var entity = await Set
                .Include(m => m.Memberships)
                .ThenInclude(m => m.MembershipType)
                .SingleOrDefaultAsync(m => m.Id == id, ct) ?? throw new NotFoundDomainException($"Entity with ID '{id}' was not found");

            ApplyUpdates(model, entity);

            var updatedEntity = await Set
                .AsNoTracking()
                .Include(m => m.Memberships)
                .ThenInclude(m => m.MembershipType)
                .SingleAsync(m => m.Id == id, ct);

            return ToDomainModel(updatedEntity);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Log(ex);
            throw;
        }
    }

    protected override void ApplyUpdates(Member model, MemberEntity entity)
    {
        // -- MEMBER DETAILS
        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.ProfileImageUrl = model.ProfileImageUrl;

        // -- MEMBERSHIPS
        var existingDatabaseMembershipIds = entity.Memberships.Select(m => m.Id).ToList();
        var currentDomainMembershipIds = model.Memberships.Select(m => m.Id).ToList();

        // Remove memberships that is no longer present in dmaon model
        foreach (MembershipEntity membershipEntity in entity.Memberships.Where(m => !currentDomainMembershipIds.Contains(m.Id)).ToList())
        {
            _context.Set<MembershipEntity>().Remove(membershipEntity);
        }

        //Adding or updating existing
        foreach (Membership membership in model.Memberships)
        {
            MembershipEntity? existingMembership = entity.Memberships.FirstOrDefault(m => m.Id == membership.Id);
            if (existingMembership is null)
            {
                // Add new membership
                var newMembershipEntity = new MembershipEntity
                {
                    Id = membership.Id,
                    MemberId = entity.Id,
                    MembershipTypeId = membership.MembershipType.Id,
                    StartDateUtc = membership.StartDateUtc,
                    EndDateUtc = membership.EndDateUtc,
                    MonthlyPrice = membership.MonthlyPrice
                };
                entity.Memberships.Add(newMembershipEntity);
            }
            else
            {
                // Update exisitng membership
                existingMembership.MembershipTypeId = membership.MembershipType.Id;
                existingMembership.StartDateUtc = membership.StartDateUtc;
                existingMembership.EndDateUtc = membership.EndDateUtc;
                existingMembership.MonthlyPrice = membership.MonthlyPrice;
            }
        }

        entity.UpdatedAtUtc = DateTime.UtcNow;
    }

    protected override Member ToDomainModel(MemberEntity entity)
    {
        var memberships = entity.Memberships?.Select(membership => ToMembershipDomainModel(membership)).ToList() ?? [];
        var model = Member.Rehydrated(entity.Id, entity.UserId, entity.FirstName, entity.LastName, entity.ProfileImageUrl,memberships);
        return model;
    }

    protected override MemberEntity ToEntity(Member model)
    {
        var memberships = model.Memberships?.Select(membership => ToMembershipEntity(membership, model.Id)).ToList() ?? [];
        var entity = new MemberEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ProfileImageUrl = model.ProfileImageUrl,
            Memberships = memberships,
            CreatedAtUtc = DateTime.UtcNow,
        };
        return entity;
    }

    private static Membership ToMembershipDomainModel(MembershipEntity entity)
    {
        var membershipType = MembershipType.Rehydrate(
            entity.MembershipType.Id,
            entity.MembershipType.Name,
            entity.MembershipType.BasePrice,
            entity.MembershipType.IsActive
        );

        return Membership.Rehydrate(
            entity.Id,
            membershipType,
            entity.StartDateUtc,
            entity.EndDateUtc,
            entity.MonthlyPrice
        );
    }

    private static MembershipEntity ToMembershipEntity(Membership model, string memberId)
    {
        return new MembershipEntity
        {
            Id = model.Id,
            MemberId = memberId,
            MembershipTypeId = model.MembershipType.Id,
            StartDateUtc = model.StartDateUtc,
            EndDateUtc = model.EndDateUtc,
            MonthlyPrice = model.MonthlyPrice
        };
    }
}
