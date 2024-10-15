using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Models
{
    public class AccountLoginDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 客户端，PC，App
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 是否刷新Token
        /// </summary>
        public bool IsRefresh { get; set; }

        /// <summary>
        /// 是否Api登录
        /// </summary>
        public bool IsApiLogin { get; set; } = true;
    }
}
