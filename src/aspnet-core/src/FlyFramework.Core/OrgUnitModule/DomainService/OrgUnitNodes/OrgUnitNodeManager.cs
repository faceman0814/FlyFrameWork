using FlyFramework.Domains;
using FlyFramework.Extentions.Object;
using FlyFramework.OrganizationalUnitModule;
using FlyFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.DomainService.OrgUnits
{
    public class OrgUnitNodeManager : GuidDomainService<OrgUnitNode>, IOrgUnitNodeManager
    {
        readonly IRepository<OrgUnitNodeGranted> _orgNodeGrantedRepo;
        public OrgUnitNodeManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._orgNodeGrantedRepo = this.GetService<IRepository<OrgUnitNodeGranted>>();
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
            nodeIdList = await _orgNodeGrantedRepo.GetAll()
                .Where(o => o.UserId == userId)
                .Select(o => o.OrgNodeId)
                .AsNoTracking()
                .ToListAsync();

            return nodeIdList;
        }

        public async Task DeleteRelation(string nodeId)
        {
            // 要删除的节点
            var node = await this.FindById(nodeId);
            // 删除节点本身
            await this.Delete(node);
            // 删除关联的授权
            await _orgNodeGrantedRepo.DeleteAsync(o => o.Id == node.Id);

            // 查询子节点
            var parentIdList = $"{node.ParentIdList}|{node.Id}";
            var deleteNodes = await this.QueryAsNoTracking.Where(o => o.ParentIdList.StartsWith(parentIdList))
                .ToListAsync();

            // 遍历删除子节点
            foreach (var item in deleteNodes)
            {
                // 删除子节点
                await this.Delete(item);

                // 删除关联的授权
                await _orgNodeGrantedRepo.DeleteAsync(o => o.Id == item.Id);
            }
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
