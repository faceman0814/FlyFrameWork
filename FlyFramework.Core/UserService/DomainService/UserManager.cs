using FlyFramework.Domain.Domains;

namespace FlyFramework.Core.UserService.DomainService
{
    public class UserManager : GuidDomainService<User>, IUserManager
    {
        public UserManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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
