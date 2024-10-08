using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Models
{
    public class AccountRegiserDto
    {
        /// <summary>
		/// 昵称
		/// </summary>
		[Required]
        [MaxLength(64)]
        public string Nickname { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Captcha { get; set; }
    }
}
