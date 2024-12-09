using System.Linq.Expressions;

namespace Planzo.Data.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includeProperties = null);

    TEntity? GetById(object id);
    Task<TEntity?> GetByIdAsync(object id);

    void Add(TEntity entity);
    Task<TEntity> AddAsync(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities);

    void Delete(object id);
    Task<TEntity?> DeleteAsync(object id);

    void Delete(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(IEnumerable<TEntity> ids);

    void DeleteRange(IEnumerable<object> entities);
    Task DeleteRangeAsync(IEnumerable<object> ids);
}