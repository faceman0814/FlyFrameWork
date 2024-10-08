using FlyFramework.Dependencys;

namespace FlyFramework.Forgery
{
    public interface IAntiForgeryManager
    //: ITransientDependency
    {
        void SetCookie();

        string GenerateToken();
    }
}
