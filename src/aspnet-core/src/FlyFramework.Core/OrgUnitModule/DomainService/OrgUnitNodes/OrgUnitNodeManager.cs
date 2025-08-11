using FlyFramework.Domains;
using FlyFramework.Extentions.Object;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodeRoles;
using FlyFramework.PermissionModule;
using FlyFramework.PermissionModule.DomainService;
using FlyFramework.Repositories;
using FlyFramework.UserModule.DomainService;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.DomainService.OrgUnitNodes
{
    public class OrgUnitNodeManager : GuidDomainService<OrgUnitNode>, IOrgUnitNodeManager
    {
        readonly IRepository<OrgUnitNodeRole, string> _orgNodeGrantedRepo;
        readonly IOrgUnitNodeRoleManager _orgNodeGrantedManager;
        readonly IUserManager _userManager;
        readonly IRolePermissionManager _rolePermissionManager;
        public OrgUnitNodeManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _orgNodeGrantedManager = GetService<IOrgUnitNodeRoleManager>();
            _userManager = GetService<IUserManager>();
            _rolePermissionManager = GetService<IRolePermissionManager>();
        }

        /// <summary>
        /// 获取用户有权访问的节点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetGrantedNodes(string userId = null)
        {
            if (!userId.HasValue())
            {
                userId = UserSession.UserId;
            }

            var user =await _userManager.FindById(userId);

            //_rolePermissionManager.QueryAsNoTracking.Where(t => t.;

            var nodeIdList = new List<string>();

            // 当前用户拥有的节点
            nodeIdList = await _orgNodeGrantedManager.QueryAsNoTracking
                //.Where(o => o.RoleId == user.)
                .Select(o => o.OrgUnitNodeId)
                .AsNoTracking()
                .ToListAsync();

            return nodeIdList;
        }


        public Task<OrgUnitNode> FindByNameAsync(string name)
        {
            return QueryAsNoTracking.FirstOrDefaultAsync(t => t.Name == name);
        }

        public override IQueryable<OrgUnitNode> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(OrgUnitNode entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(OrgUnitNode entity)
        {
            return Task.CompletedTask;
        }
    }
}
