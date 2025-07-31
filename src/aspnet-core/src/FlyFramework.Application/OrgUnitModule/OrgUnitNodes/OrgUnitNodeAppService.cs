using FlyFramework.ApplicationServices;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.OrgUnitModule.DomainService.OrgUnitNodes;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;
using FlyFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes
{
    public class OrgUnitNodeAppService : ApplicationService, IOrgUnitNodeAppService
    {
        private readonly IOrgUnitNodeManager _orgUnitNodeManager;
        private readonly IRepository<OrgUnitNode, string> _repository;

        public OrgUnitNodeAppService(IOrgUnitNodeManager orgUnitNodeManager, IRepository<OrgUnitNode, string> repository)
        {
            _orgUnitNodeManager = orgUnitNodeManager;
            _repository = repository;
        }

        /// <summary>
        /// 创建或更新组织机构节点信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdate(CreateOrUpdateOrgUnitNodeInput input)
        {
            if (input.OrgUnitNode.Id.HasValue())
            {
                await Update(input.OrgUnitNode);
            }
            else
            {
                await Create(input.OrgUnitNode);
            }
        }

        /// <summary>
        /// 删除组织机构节点信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Delete(EntityDto<string> input)
        {
            await _orgUnitNodeManager.Delete(input.Id);
        }

        /// <summary>
        /// 获取组织机构节点编辑信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OrgUnitNodeForEditOutput> GetForEdit(EntityDto<string> input)
        {
            var output = new OrgUnitNodeForEditOutput();
            var entity = await _orgUnitNodeManager.FindById(input.Id);
            output.OrgUnitNode = ObjectMapper.Map<OrgUnitNodeEditDto>(entity);
            return output;
        }

        #region 私有方法
        private async Task Create(OrgUnitNodeEditDto input)
        {
            var entity = ObjectMapper.Map<OrgUnitNode>(input);
            await _orgUnitNodeManager.Create(entity);
        }

        private async Task Update(OrgUnitNodeEditDto input)
        {
            var entity = await _orgUnitNodeManager.FindById(input.Id);
            entity = ObjectMapper.Map<OrgUnitNode>(input);
            await _orgUnitNodeManager.Update(entity);
        }
        #endregion

        #region 树操作接口

        /// <summary>
        /// 获取树数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<OrgUnitNodeListDto>> GetTree(GetOrgUnitNodesInput input)
        {
            var result = new List<OrgUnitNodeListDto>();
            // 当前用户拥有的节点
            var nodeIdList = await _orgUnitNodeManager.GetGrantedNodes();

            // 查询子节点总数，同时也要过滤有权限的子节点
            var resultNodeIdList = result.Select(o => o.Id).ToList();
            var totalChildMap = await _orgUnitNodeManager.Query
                 .Where(o =>
                 nodeIdList.Contains(o.Id) &&
                 resultNodeIdList.Contains(o.ParentId))
                 .GroupBy(o => o.ParentId)
                 .Select(o => new
                 {
                     ParentId = o.Key,
                     Count = o.Count(),
                 })
                 .ToDictionaryAsync(o => o.ParentId, o => o.Count);
            foreach (var item in result)
            {
                if (totalChildMap.TryGetValue(item.Id, out var totalChild))
                {
                    item.TotalChild = totalChild;
                }
            }

            return result;
        }

        #endregion
    }
}
