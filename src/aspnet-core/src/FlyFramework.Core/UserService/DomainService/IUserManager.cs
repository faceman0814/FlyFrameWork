using FlyFramework.Domain.Domains;

namespace FlyFramework.Core.UserService.DomainService
{
    public interface IUserManager : IGuidDomainService<User>
    {
        Task<User> FindByNameAsync(string userName);
    }
}
