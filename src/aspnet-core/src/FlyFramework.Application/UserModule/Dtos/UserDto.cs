using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFramework.UserModule.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        [Column("用户名称")]
        public string Name { get; set; }
        public string Password { get; set; }
        [Column("邮箱")]
        public string Email { get; set; }
        [Column("别名")]
        public string FullName { get; set; }
        [Column("用户名")]
        public string UserName { get; set; }
        [Column("手机号码")]
        public string PhoneNumber { get; set; }
        [Column("是否启用")]
        public bool IsActive { get; set; }
        [Column("创建时间")]
        public DateTime CreationTime { get; set; }
    }
}
