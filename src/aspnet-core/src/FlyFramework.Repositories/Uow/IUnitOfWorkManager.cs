namespace FlyFramework.Uow
{
    public interface IUnitOfWorkManager
    //: IScopedDependency
    {
        IUnitOfWork Current { get; }

        IUnitOfWork Begin();

        IUnitOfWork Reserve();

    }
}
