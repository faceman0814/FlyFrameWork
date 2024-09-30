using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Models
{

    public class AuthenticateResultModel
    {
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// 加密后的密码
        /// </summary>
        public string Password { get; set; }
        //
        // 摘要:
        //     访问令牌
        public string AccessToken { get; set; }

        //
        // 摘要:
        //     加密访问令牌
        public string EncryptedAccessToken { get; set; }

        //
        // 摘要:
        //     刷新令牌
        public string RefreshToken { get; set; }

        //
        // 摘要:
        //     用户编号
        public string UserId { get; set; }

        //
        // 摘要:
        //     是否需要重置密码
        public bool ShouldResetPassword { get; set; }

        //
        // 摘要:
        //     密码重置代码
        public string PasswordResetCode { get; set; }

        //
        // 摘要:
        //     刷新令牌过期时间
        public DateTimeOffset RefreshTokenExpire { get; set; }

        //
        // 摘要:
        //     登录成功后的跳转地址。
        public string ReturnUrl { get; set; }

        //
        // 摘要:
        //     是否需要进行账号绑定激活。
        public bool WaitingForActivation { get; set; }
    }
}
