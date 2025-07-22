using FlyFramework.Entities;
using FlyFramework.UserModule;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace FlyFramework.OrgUnitModule
{
    /// <summary>
    /// 部门角色关系
    /// </summary>
    public class OrgUnitNodeRole : CreationAuditedEntity<string>, IMayHaveTenant
    {
        /// <summary>
        /// 节点关联的数据Id
        /// <see cref="OrgUnitNode.Id"/>
        /// </summary>
        [Comment("节点关联的数据Id")]
        public string OrgUnitNodeId { get; set; }

        /// <summary>
        /// 节点关联的数据Id
        /// <see cref="Role.Id"/>
        /// </summary>
        [Comment("角色Id")]
        public string RoleId { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Comment("租户Id")]
        public string TenantId { get; set; }
    }
}
