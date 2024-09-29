using FlyFramework.Domains;
using FlyFramework.UserModule;

using Microsoft.EntityFrameworkCore;

using ServiceStack;

using System;
using System.Linq;
using System.Threading.Tasks;
namespace FlyFramework.UserModule.DomainService
{
    public class UserManager : GuidDomainService<User>, IUserManager
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<User> CreateUserAsync(User user)
        {
            user.NormalizedUserName = user.UserName.ToUpper();
            user.NormalizedEmail = user.Email.ToUpper();
            user.TwoFactorEnabled = false;
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = false;
            return Create(user);
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return QueryAsNoTracking.FirstOrDefaultAsync(t => t.UserName == userName);
        }

        public override IQueryable<User> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(User entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(User entity)
        {
            return Task.CompletedTask;
        }
    }
}
