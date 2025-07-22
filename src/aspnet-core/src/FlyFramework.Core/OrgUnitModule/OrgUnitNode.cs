using FlyFramework.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule
{
    /// <summary>
    /// 组织机构节点
    /// </summary>
    public class OrgUnitNode : FullAuditedEntity<string>, IMayHaveTenant
    {
        /// <summary>
        /// 组织机构名称
        /// </summary>
        [Comment("组织机构名称")]
        public string Name { get; set; }

        /// <summary>
        /// 部门负责人ID
        /// </summary>
        [Comment("部门负责人ID")]
        public string LeaderId { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [Comment("父级Id")]
        public string ParentId { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Comment("租户ID")]
        public string TenantId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Comment("状态")]
        public OrgUnitNodeStatus Status { get; set; }
    }

    public enum OrgUnitNodeStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Enabled = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disabled = 0
    }
}
