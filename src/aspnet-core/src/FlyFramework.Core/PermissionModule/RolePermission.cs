using FlyFramework.Entities;
namespace FlyFramework.PermissionModule
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    public class RolePermission : FullAuditedEntity<string>, IMustHaveTenant
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 权限Id
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public string TenantId { get; set; }

    }
}
