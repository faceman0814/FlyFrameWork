using FlyFramework.Attributes;
using FlyFramework.Dtos;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFramework.UserModule.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class CreateOrUpdateUserInput
    {
        [Required]
        public UserDto User { get; set; }
    }

    public class UserListDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        [Column("别名")]
        public string FullName { get; set; }

        [Column("用户名")]
        public string UserName { get; set; }

        [Column("邮箱")]
        public string Email { get; set; }

        [Column("手机号码")]
        public string PhoneNumber { get; set; }

        [Column("组织单元")]
        public string OrgUnitNodeName { get; set; }

        [Column("是否启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        [Column("创建时间")]
        public DateTime CreationTime { get; set; }

        [Column("操作")]
        [Sort(99)]
        public String Opertion { get; set; }
    }

    public class GetUsersInput : GetListInput
    {

    }

}
