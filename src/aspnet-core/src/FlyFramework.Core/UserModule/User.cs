using FlyFramework.Entities;
using FlyFramework.TenantModule;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
namespace FlyFramework.UserModule
{
    public class User : IdentityUser<string>, IFullAuditedEntity<string>, IMustHaveTenant
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
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

        public object[] GetKeys()
        {
            return new string?[] { Id };
        }
    }
}
