namespace FlyFramework.Utilities.JWTTokens
{
    public class JwtBearerModel : IJwtBearerModel
    {
        /// <summary>
        /// 发布者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接受者
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
        /// <summary>
        /// 刷新令牌过期时间（分钟）
        /// </summary>
        public int RefreshTokenExpiresMinutes { get; set; }
    }
}