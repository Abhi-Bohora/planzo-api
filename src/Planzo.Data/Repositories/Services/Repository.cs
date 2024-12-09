using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Planzo.Data.Repositories.Interfaces;

namespace Planzo.Data.Repositories.Services;

public class Repository<TEntity, TContext>(TContext context)
    : IDisposable, IRepository<TEntity>, IAsyncDisposable
    where TEntity : class where TContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true)
            .ConfigureAwait(false);
        GC.SuppressFinalize(this);
        await context.DisposeAsync()
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        context.Dispose();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null) query = query.Where(filter);

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null) query = query.Where(filter);

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        return orderBy != null ? await orderBy(query).FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
    }

    public TEntity? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id)
            .ConfigureAwait(false);
    }

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity)
            .ConfigureAwait(false);
        return entity;
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Delete(object id)
    {
        var entityToDelete = _dbSet.Find(id);
        if (entityToDelete != null)
            Delete(entityToDelete);
    }

    public async Task<TEntity?> DeleteAsync(object id)
    {
        var entityToDelete = await _dbSet.FindAsync(id)
            .ConfigureAwait(false);

        if (entityToDelete != null)
            await DeleteAsync(entityToDelete)
                .ConfigureAwait(false);

        return entityToDelete;
    }

    public void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached) _dbSet.Attach(entityToDelete);

        _dbSet.Remove(entityToDelete);
    }

    public Task<TEntity> DeleteAsync(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached) _dbSet.Attach(entityToDelete);

        _dbSet.Remove(entityToDelete);
        return Task.FromResult(entityToDelete);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public void DeleteRange(IEnumerable<object> ids)
    {
        var toDelete = _dbSet.Where(entity => ids.Contains(entity)).ToList();
        _dbSet.RemoveRange(toDelete);
    }

    public Task DeleteRangeAsync(IEnumerable<object> ids)
    {
        var toDelete = _dbSet.Where(entity => ids.Contains(entity)).ToList();
        _dbSet.RemoveRange(toDelete);
        return Task.FromResult(toDelete.FirstOrDefault());
    }

    public void Update(TEntity entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public Task<TEntity> UpdateAsync(TEntity entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
        return Task.FromResult(entityToUpdate);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities)
            .ConfigureAwait(false);
    }

    private void Dispose(bool isDisposed)
    {
        if (!_disposed)
            if (isDisposed)
                context.Dispose();

        _disposed = true;
    }

    private async ValueTask DisposeAsync(bool isDisposed)
    {
        if (!_disposed)
            if (isDisposed)
                await context.DisposeAsync()
                    .ConfigureAwait(false);

        _disposed = true;
    }

    public void UpdateRange(IEnumerable<TEntity> entityToUpdate)
    {
        _dbSet.UpdateRange(entityToUpdate);
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entitiesToUpdate)
    {
        var toUpdate = entitiesToUpdate.ToList();
        _dbSet.UpdateRange(toUpdate);
        return Task.FromResult(toUpdate);
    }
}