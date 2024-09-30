using System.ComponentModel.DataAnnotations;

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
        /// 第三方登陆提供者数据Id
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// 第三方登陆预授权编码
        /// </summary>
        public string ProviderCode { get; set; }

        /// <summary>
        /// 返回Url
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 客户端，PC，App
        /// </summary>
        public string ClientType { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        public bool RememberMe { get; set; }
        /// <summary>
        /// 是否加密
        /// </summary>
        public bool IsEncrypt { get; set; }
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