using FaceMan.DynamicWebAPI;

using FlyFramework.Dtos;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes
{
    /// <summary>
    /// 组织机构节点服务接口
    /// </summary>
    public interface IOrgUnitNodeAppService : IApplicationService
    {
        /// <summary>
        /// 获取组织机构节点树形结构
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<OrgUnitNodeListDto>> GetTree(GetOrgUnitNodesInput input);

        /// <summary>
        /// 获取组织机构节点编辑信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<OrgUnitNodeForEditOutput> GetForEdit(EntityDto<string> input);

        /// <summary>
        /// 创建或更新组织机构节点信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdate(CreateOrUpdateOrgUnitNodeInput input);

        /// <summary>
        /// 删除组织机构节点信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(EntityDto<string> input);
    }
}
