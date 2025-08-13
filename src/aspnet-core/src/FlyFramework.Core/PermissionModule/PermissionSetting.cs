using FlyFramework.Entities;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFramework.PermissionModule;
/// <summary>
/// 权限表
/// </summary>
[Table("Permissions")]
public class PermissionSetting : CreationAuditedEntity<string>, IMayHaveTenant
{
    public const int MaxNameLength = 128;

    public virtual string TenantId { get; set; }

    [Required]
    [StringLength(128)]
    public virtual string Name { get; set; }

    public virtual bool IsGranted { get; set; }

    protected PermissionSetting()
    {
        IsGranted = true;
    }
}
