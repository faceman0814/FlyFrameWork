using FlyFramework.Entities;

using Masuit.Tools.Models;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
namespace FlyFramework.UserModule
{
    public class User : IdentityUser<string>, IFullAuditedEntity<string>, IMustHaveTenant
    {
        //默认密码
        public const string DefaultPassword = "bb123456";

        //手机号最大长度 
        public new const int MaxPhoneNumberLength = 18;

        //真实姓名最大长度
        public const int MaxRealNameLength = 1024;

        //工号最大长度
        public const int MaxEmployeeNumberLength = 1024;
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool NeedToChangeThePassword { get; set; }
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

        public virtual void SetNormalizedNames()
        {
            NormalizedUserName = UserName.ToUpperInvariant();
            NormalizedEmail = Email.ToUpperInvariant();
        }
    }
}
