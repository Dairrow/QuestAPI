using Data.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;
using System.Linq.Expressions;

namespace Repository.Implementations;

public class BaseRepository<TEntity>
    : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly AppDbContext Context;

    protected readonly DbSet<TEntity> DbSet;


    public BaseRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }


    public virtual async Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(
            [id],
            cancellationToken);
    }


    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


    public virtual async Task<IReadOnlyCollection<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }


    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(predicate, cancellationToken);
    }


    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(
            entity,
            cancellationToken);
    }


    public virtual void Update(
        TEntity entity)
    {
        DbSet.Update(entity);
    }


    public virtual void Delete(
        TEntity entity)
    {
        DbSet.Remove(entity);
    }


    public virtual async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context
            .SaveChangesAsync(cancellationToken);
    }
}