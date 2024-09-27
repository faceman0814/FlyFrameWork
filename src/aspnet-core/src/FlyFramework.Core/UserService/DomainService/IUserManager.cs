using FlyFramework.Domain.Domains;

using System.Threading.Tasks;
namespace FlyFramework.Core.UserService.DomainService
{
    public interface IUserManager : IGuidDomainService<User>
    {
        Task<User> FindByNameAsync(string userName);
    }
}
