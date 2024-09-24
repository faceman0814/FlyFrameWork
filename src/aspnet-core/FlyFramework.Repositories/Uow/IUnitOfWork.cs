using FlyFramework.Common.Dependencys;

namespace FlyFramework.Repositories.Uow
{
    public interface IUnitOfWork : IScopedDependency
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task BeginAsync(CancellationToken cancellationToken = default);
    }
}
