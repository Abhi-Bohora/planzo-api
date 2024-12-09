using Microsoft.EntityFrameworkCore;
using Planzo.Data.Repositories.Interfaces;
using Planzo.Data.Repositories.Services;

namespace Planzo.Data.UnitOfWork;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork,
    IDisposable,
    IAsyncDisposable where TContext : DbContext
{
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

    public IRepository<T> Repository<T>() where T : class
    {
        return new Repository<T, TContext>(context);
    }

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
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
}