using FlyFramework.Domains;
using FlyFramework.OrganizationalUnitModule;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.DomainService.OrgUnits
{
    public interface IOrgUnitNodeManager : IGuidDomainService<OrgUnitNode>
    {
        /// <summary>
        /// 获取用户能访问的节点id列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="nodeType">节点类型</param>
        /// <returns></returns>
        Task<List<string>> GetGrantedNodes(string userId = null);
    }
}
