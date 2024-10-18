using FlyFramework.Entities;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFramework.OrganizationalUnitModule
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public class OrgUnit : FullAuditedEntity<string>, IMayHaveTenant
    {
        public string TenantId { get; set; }

        /// <summary>
        /// 组织机构名称
        /// </summary>
        [Comment("组织机构名称")]
        public string Name { get; set; }

        /// <summary>
        /// 父节点Id
        /// <see cref="OrgUnitNode.ParentId"/>
        /// </summary>
        [Comment("父节点Id")]
        public string ParentId { get; set; }

        /// <summary>
        /// 父级组织机构Id
        /// </summary>
        [Comment("父级数据id集合，aaa|bbb|ccc")]
        public string ParentIdList { get; set; }

        [ForeignKey("ParentId")]
        public virtual OrgUnit Parent { get; set; }
    }
}
