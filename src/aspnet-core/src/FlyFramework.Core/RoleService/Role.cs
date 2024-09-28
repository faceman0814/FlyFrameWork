using FlyFramework.Entities;

using Microsoft.AspNetCore.Identity;

using System;
namespace FlyFramework.RoleService
{
    public class Role : IdentityRole<string>, IFullAuditedEntity<string>
    {
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

        public bool IsTransient()
        {
            throw new NotImplementedException();
        }
    }
}
