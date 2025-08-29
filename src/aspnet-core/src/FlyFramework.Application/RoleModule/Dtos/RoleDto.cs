using FlyFramework.Attributes;
using FlyFramework.Dtos;
using FlyFramework.UserModule.Dtos;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FlyFramework.FlyFrameworkConfigs;

namespace FlyFramework.RoleModule.Dtos
{
    public class RoleDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否静态角色，静态角色无法删除，无法更改其名称。它们可以通过编程方式使用。
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 该角色是否将被分配给新用户？
        /// </summary>
        public bool IsDefault { get; set; }
    }

    public class RoleListDto
    {
        public string Id { get; set; }

        [Column("角色编码")]
        public string Name { get; set; }

        [Column("角色名称")]
        public string DisplayName { get; set; }

        [Column("是否静态角色")]
        public bool IsStatic { get; set; }

        [Column("是否默认角色")]
        public bool IsDefault { get; set; }

        [Column("创建时间")]
        public DateTime CreationTime { get; set; }
    }

    public class GetRolesInput : GetListInput
    {

    }

    public class CreateOrUpdateRoleInput
    {
        [Required]
        public RoleDto Role { get; set; }
    }
}
