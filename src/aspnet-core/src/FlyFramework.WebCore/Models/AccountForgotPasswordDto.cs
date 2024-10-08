using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models
{
    public class AccountForgotPasswordDto
    {
        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Captcha { get; set; }
    }
}
