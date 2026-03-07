using Domain.Abstractions.Logging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Persistence.Repositories;
using Domain.Exceptions.Custom;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EFC.Repositories;

public abstract class RepositoryBase<TDomainModell, TId, TEntity, TContext>(TContext context, ILogger logger) 
    : IRepositoryBase<TDomainModell, TId>
    where TContext : DbContext
    where TEntity : class, IEntity<TId>    
{
    protected readonly TContext _context = context;
    protected readonly ILogger _logger = logger;
    protected DbSet<TEntity> Set => _context.Set<TEntity>();

    public virtual async Task<TDomainModell?> AddAsync(TDomainModell model, CancellationToken ct = default)
    {
        try
        {
            if (model is null)
                throw new NullDomainException("Domain model must be provided.");

            var entity = ToEntity(model);

            await Set.AddAsync(entity, ct);

            return ToDomainModel(entity);
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

    public virtual async Task<IReadOnlyList<TDomainModell>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var entities = await Set.AsNoTracking().ToListAsync(ct);
            return [..entities.Select(ToDomainModel)];
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.Log(ex);
            throw;
        }
    }

    public virtual async Task<TDomainModell?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        try
        {
            var entity = await Set.SingleOrDefaultAsync(e => e.Id!.Equals(id), ct);
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

    public virtual async Task<bool> RemoveByIdAsync(TId id, CancellationToken ct = default)
    {
        try
        {
            var entity = await Set.SingleOrDefaultAsync(e => e.Id!.Equals(id), ct);
            if(entity is null) return false;

            Set.Remove(entity);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.Log(ex);
            throw;
        }
    }

    public virtual async Task<TDomainModell?> UpdateAsync(TId id, TDomainModell model, CancellationToken ct = default)
    {
        try
        {
            if (model is null)
                throw new NullDomainException("Domain model must be provided.");

            var entity = await Set.FindAsync([id], ct) ?? throw new NotFoundDomainException($"Entity with ID '{id}' was not found");

            ApplyUpdates(model, entity);
            
            return ToDomainModel(entity);
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

    protected abstract void ApplyUpdates(TDomainModell model, TEntity entity);
    protected abstract TDomainModell ToDomainModel(TEntity entity);
    protected abstract TEntity ToEntity(TDomainModell model);
}
