using FlyFramework.ApplicationServices;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.OrganizationalUnitModule;
using FlyFramework.OrgUnitModule.DomainService.OrgUnits;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;
using FlyFramework.OrgUnitModule.OrgUnits.Dtos;
using FlyFramework.UserModule;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes
{
    public class OrgUnitNodeAppService : ApplicationService, IOrgUnitNodeAppService
    {
        private readonly IOrgUnitManager _orgUnitManager;
        private readonly IOrgUnitNodeManager _orgUnitNodeManager;

        public OrgUnitNodeAppService(IFlyFrameworkLazy flyFrameworkLazy)
        {
            _orgUnitManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitManager>().Value;
            _orgUnitNodeManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitNodeManager>().Value;
        }

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

        public async Task Delete(EntityDto<string> input)
        {
            await _orgUnitNodeManager.Delete(input.Id);
        }

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
        [HttpPost]
        //[AbpAuthorize(KnightOrgNodePermissions.Node)]
        public async Task<List<OrgUnitNodeListDto>> GetTree(GetOrgUnitsInput input)
        {
            var result = new List<OrgUnitNodeListDto>();
            // 当前用户拥有的节点
            var nodeIdList = await _orgUnitNodeManager.GetGrantedNodes();

            // 如果有筛选条件
            if (input.FilterText.HasValue())
            {
                // 筛选过的节点信息
                var filterNodeList = await _orgUnitNodeManager.QueryAsNoTracking
                    .WhereIfThenElse(input.OrgUnitNodeId.HasValue(),
                            o => nodeIdList.Contains(o.Id) && o.Name.Contains(input.FilterText) && o.Id == input.OrgUnitNodeId,
                            o => nodeIdList.Contains(o.Id) && o.Name.Contains(input.FilterText)
                            )
                    .Select(o => new { o.Id, o.ParentIdList })
                    .ToListAsync();

                var nodeIdList2 = filterNodeList.Select(o => o.Id).ToList();

                // 父级节点Id集合
                var nodeParentIdList = filterNodeList.Select(o => o.ParentIdList).ToList();
                var parentIdDict = new Dictionary<string, bool>();
                foreach (var item in nodeParentIdList)
                {
                    if (!item.HasValue())
                    {
                        continue;
                    }

                    var parentIdArray = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var parentId in parentIdArray)
                    {
                        if (parentIdDict.ContainsKey(parentId))
                        {
                            continue;
                        }
                        parentIdDict[parentId] = true;
                    }
                }
                var nodeIdList3 = parentIdDict.Keys.ToList().Concat(nodeIdList2).Distinct().ToList();

                // 只获取顶级节点
                var nodeList = await _orgUnitNodeManager.QueryAsNoTracking
                    .Where(o => o.ParentId == input.ParentOrgUnitNodeId && nodeIdList3.Contains(o.Id))
                    .ToListAsync();
                // 最终数据
                result = ObjectMapper.Map<List<OrgUnitNodeListDto>>(nodeList);
            }
            // 默认展开
            else
            {
                var nodeList = await _orgUnitNodeManager.QueryAsNoTracking
                        .WhereIfThenElse(input.OrgUnitNodeId.HasValue(),
                            o => o.ParentId == input.ParentOrgUnitNodeId && nodeIdList.Contains(o.Id) && o.Id == input.OrgUnitNodeId,
                            o => o.ParentId == input.ParentOrgUnitNodeId && nodeIdList.Contains(o.Id)
                            )
                        .ToListAsync();

                // 最终数据
                result = ObjectMapper.Map<List<OrgUnitNodeListDto>>(nodeList);
            }


            // 查询子节点总数，同时也要过滤有权限的子节点
            var resultNodeIdList = result.Select(o => o.Id).ToList();
            var totalChildMap = await _orgUnitNodeManager.Query
                 .Where(o => nodeIdList.Contains(o.Id) && resultNodeIdList.Contains(o.ParentId))
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
