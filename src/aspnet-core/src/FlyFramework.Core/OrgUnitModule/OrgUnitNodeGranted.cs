using FlyFramework.Entities;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace FlyFramework.OrganizationalUnitModule
{
    /// <summary>
    /// 组织节点授权
    /// </summary>
    public class OrgUnitNodeGranted : CreationAuditedEntity<string>, IMayHaveTenant
    {
        /// <summary>
        /// 系统用户Id
        /// </summary>
        [Comment("系统用户Id")]
        [StringLength(32)]
        public string UserId { get; set; }

        /// <summary>
        /// 节点关联的数据Id
        /// <see cref="OrgUnitNode.Id"/>
        /// </summary>
        [Comment("节点关联的数据Id")]
        [StringLength(32)]
        public string OrgNodeId { get; set; }

        public string TenantId { get; set; }
    }
}
