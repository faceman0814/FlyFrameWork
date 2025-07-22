using FlyFramework.Entities;

using Masuit.Tools.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
namespace FlyFramework.UserModule
{
    public class User : IdentityUser<string>, IFullAuditedEntity<string>, IMustHaveTenant
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [Comment("用户名称")]
        public string FullName { get; set; }

        /// <summary>
        /// 组织单元Id
        /// </summary>
        [Comment("组织单元Id")]
        public string OrgUnitNodeId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Comment("是否启用")]
        public bool IsActive { get; set; }


        /// <summary>
        /// 是否需要修改密码
        /// </summary>
        [Comment("是否需要修改密码")]
        public bool NeedToChangeThePassword { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [Comment("是否已删除")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除人Id
        /// </summary>
        [Comment("删除人Id")]
        public string DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Comment("删除时间")]
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 删除人名称
        /// </summary>
        [Comment("删除人名称")]
        public string DeleterUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Comment("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 最后修改人名称
        /// </summary>
        [Comment("最后修改人名称")]
        public string LastModifierUserName { get; set; }

        /// <summary>
        /// 最后修改人Id
        /// </summary>
        [Comment("最后修改人Id")]
        public string LastModifierUserId { get; set; }

        /// <summary>
        /// 并发令牌
        /// </summary>
        [Comment("并发令牌")]
        public string ConcurrencyToken { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [Comment("创建人名称")]
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [Comment("创建人Id")]
        public string CreatorUserId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Comment("租户Id")]
        public string TenantId { get; set; }

        public bool IsTransient()
        {
            if (EqualityComparer<string>.Default.Equals(Id, default))
            {
                return true;
            }

            if (typeof(string) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(string) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }

        public virtual void SetNormalizedNames()
        {
            NormalizedUserName = UserName.ToUpperInvariant();
            NormalizedEmail = Email.ToUpperInvariant();
        }
    }
}
