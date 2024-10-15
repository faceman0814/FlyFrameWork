using FlyFramework.Entities;

using Microsoft.AspNetCore.Identity;

using System;
namespace FlyFramework.UserModule
{
    public class Role : IdentityRole<string>, IFullAuditedEntity<string>, IMayHaveTenant
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 这是静态角色吗？静态角色无法删除，无法更改其名称。它们可以通过编程方式使用。
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// 该角色是否将被分配给新用户？
        /// </summary>
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }
        public string DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
        public string LastModifierUserId { get; set; }
        public string ConcurrencyToken { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public string CreatorUserId { get; set; }

        public string TenantId { get; set; }
        public bool IsTransient()
        {
            throw new NotImplementedException();
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
