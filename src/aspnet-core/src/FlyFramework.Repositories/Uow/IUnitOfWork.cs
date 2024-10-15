using FlyFramework.Dependencys;

using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework.Uow
{
    public interface IUnitOfWork : IScopedDependency, IDisposable
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task BeginAsync(CancellationToken cancellationToken = default);

        DbContext GetDbContext();
        Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default);
    }
}
