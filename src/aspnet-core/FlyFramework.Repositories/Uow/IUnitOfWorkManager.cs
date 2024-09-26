using FlyFramework.Common.Dependencys;

namespace FlyFramework.Repositories.Uow
{
    public interface IUnitOfWorkManager
    //: IScopedDependency
    {
        IUnitOfWork? Current { get; }

        IUnitOfWork Begin();

        IUnitOfWork Reserve();

    }
}
