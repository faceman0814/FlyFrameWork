using System;

namespace FlyFramework.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly FlyFrameworkDbContext _context;
        public HostRoleAndUserCreator(FlyFrameworkDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var helper = new FlyFrameworkCreatorHelper();

            helper.SetContext(null, _context);
            //创建Host管理员角色
            var roleId = helper.CreateRole("admin", "系统管理员", false);
          
            //创建用户
            helper.CreateUser(roleId, "admin");
        }
    }
}
