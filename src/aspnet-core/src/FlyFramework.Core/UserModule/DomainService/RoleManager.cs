using FlyFramework.Domains;
using FlyFramework.ErrorExceptions;
using FlyFramework.Repositories;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.UserModule.DomainService
{
    public class RoleManager : GuidDomainService<Role>, IRoleManager
    {
        private readonly IRepository<UserRole, string> _userRepository;
        public RoleManager(IServiceProvider serviceProvider, IRepository<UserRole, string> userRepository) : base(serviceProvider)
        {
            _userRepository = userRepository;
        }

        public override IQueryable<Role> GetIncludeQuery()
        {
            return this.Query;
        }

        public override Task ValidateOnCreateOrUpdate(Role entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(Role entity)
        {
            var isUser = _userRepository.GetAll().Any(t => t.RoleId.Equals(entity.Id));
            if (isUser)
            {
                throw new UserFriendlyException("角色已分配用户，不能删除");
            }
            return Task.CompletedTask;
        }
    }
}
