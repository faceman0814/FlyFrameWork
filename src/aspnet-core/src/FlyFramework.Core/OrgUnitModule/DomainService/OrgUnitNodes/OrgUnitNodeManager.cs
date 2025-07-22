using FlyFramework.Domains;
using FlyFramework.Extentions.Object;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodeRoles;
using FlyFramework.Repositories;

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
        public OrgUnitNodeManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _orgNodeGrantedManager = GetService<IOrgUnitNodeRoleManager>();
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
            var nodeIdList = new List<string>();

            // 当前用户拥有的节点
            nodeIdList = await _orgNodeGrantedManager.QueryAsNoTracking
                .Where(o => o.UserId == userId)
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
