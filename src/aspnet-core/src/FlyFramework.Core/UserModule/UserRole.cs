using FlyFramework.Entities;

namespace FlyFramework.UserModule
{
    public class UserRole : FullAuditedEntity<string>
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string TenantId { get; set; }
    }
}
