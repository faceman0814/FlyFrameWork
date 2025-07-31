using FlyFramework.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
namespace FlyFramework.UserModule
{
    public class Role : IdentityRole<string>, IFullAuditedEntity<string>, IMayHaveTenant
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        [Comment("显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否静态角色，静态角色无法删除，无法更改其名称。它们可以通过编程方式使用。
        /// </summary>
        [Comment("是否静态角色")]
        public bool IsStatic { get; set; }

        /// <summary>
        /// 该角色是否将被分配给新用户？
        /// </summary>
        [Comment("是否默认角色")]
        public bool IsDefault { get; set; }

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

        public Role()
        {

        }

        public Role(string tenantId, string name, string displayName)
        {
            Id = Guid.NewGuid().ToString("N");
            TenantId = tenantId;
            Name = name;
            DisplayName = displayName;
        }
    }
}
