using FlyFramework.UserModule;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var roleId = helper.CreateRole("Admin", "系统管理员", false);
            //helper.CreateRolePermissions(
            //    roleId,
            //    TemplateAuthorizationProviderHelper.GetAllAuthorizationProviders(tenantId)
            //    );
            helper.CreateUser(roleId, "admin");
        }
    }
}
