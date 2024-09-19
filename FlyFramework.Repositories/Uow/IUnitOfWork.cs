using FlyFramework.Common.Dependencys;

namespace FlyFramework.Repositories.Uow
{
    public interface IUnitOfWork : ITransientDependency
    {
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task BeginTransactionAsync();
    }
}
