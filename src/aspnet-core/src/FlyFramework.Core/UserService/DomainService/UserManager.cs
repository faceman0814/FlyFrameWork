using FlyFramework.Domain.Domains;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;
namespace FlyFramework.Core.UserService.DomainService
{
    public class UserManager : GuidDomainService<User>, IUserManager
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return this.QueryAsNoTracking.FirstOrDefaultAsync(t => t.UserName == userName);
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
