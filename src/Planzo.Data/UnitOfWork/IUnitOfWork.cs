using Planzo.Data.Repositories.Interfaces;

namespace Planzo.Data.UnitOfWork;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
}