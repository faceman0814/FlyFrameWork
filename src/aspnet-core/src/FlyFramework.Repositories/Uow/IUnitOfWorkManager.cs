using System;

namespace FlyFramework.Uow
{
    public interface IUnitOfWorkManager : IDisposable
    //: IScopedDependency
    {
        IUnitOfWork Current { get; }

        IUnitOfWork Begin();

        IUnitOfWork Reserve();

    }
}
