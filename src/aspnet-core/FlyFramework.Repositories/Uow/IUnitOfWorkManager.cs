using FlyFramework.Common.Dependencys;

namespace FlyFramework.Repositories.Uow
{
    public interface IUnitOfWorkManager : IScopedDependency
    {
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task BeginTransactionAsync();
    }
}
