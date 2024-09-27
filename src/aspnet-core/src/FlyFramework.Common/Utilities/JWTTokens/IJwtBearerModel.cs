using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.JWTTokens
{
    public interface IJwtBearerModel
    {
        /// <summary>
        /// 发布者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 过期时间（分钟）
        /// </summary>
        public int AccessTokenExpiresMinutes { get; set; }
    }
}
