using FlyFramework.Attributes;
using FlyFramework.Dtos;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using static FlyFramework.FlyFrameworkConfigs;

namespace FlyFramework.UserModule.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        [Column("别名")]
        public string FullName { get; set; }
        [Column("用户名")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [Column("邮箱")]
        public string Email { get; set; }
        [Column("手机号码")]
        public string PhoneNumber { get; set; }
        [Column("是否启用")]
        public bool IsActive { get; set; }
        [Column("创建时间")]
        public DateTime CreationTime { get; set; }
    }

    public class CreateOrUpdateUserParam
    {
        public int? id { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        public UserDto Entity { get; set; }
    }

    public class UserListDto : UserDto
    {
        /// <summary>
        /// 操作
        /// </summary>
        [Column("操作")]
        [Sort(99)]
        public String Opertion { get; set; }
    }

    public class GetUsersInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
		/// 正常化排序使用
		/// </summary>
		public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }
        }
    }

}
