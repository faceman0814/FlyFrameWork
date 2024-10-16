using FlyFramework.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrganizationalUnitModule
{
    /// <summary>
    /// 组织机构节点
    /// </summary>
    public class OrgUnitNode : FullAuditedEntity<string>, IMayHaveTenant
    {
        public string TenantId { get; set; }
        /// <summary>
        /// 组织机构名称
        /// </summary>
        [Comment("节点显示名称")]
        public string Name { get; set; }

        /// <summary>
        /// 父级组织机构Id
        /// </summary>
        [Comment("父级数据id集合，aaa|bbb|ccc")]
        public string ParentIdList { get; set; }

        /// <summary>
        /// 节点关联数据的父级数据Id
        /// </summary>
        [Comment("节点关联数据的父级数据Id")]
        [StringLength(32)]
        public string ParentId { get; set; }
    }
}
