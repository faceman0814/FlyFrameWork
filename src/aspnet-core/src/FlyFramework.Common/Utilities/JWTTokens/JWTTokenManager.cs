using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Utilities.JWTTokens
{
    public class JWTTokenManager : IJWTTokenManager
    {
        private readonly IJwtBearerModel _jwtModel;
        private readonly IConfiguration _configuration;

        public JWTTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtModel = _configuration.GetSection("JwtBearer").Get<JwtBearerModel>();

        }

        public string GenerateToken(List<Claim> claims, DateTime expires)
        {
            return CreateTokenString(claims, expires);
        }

        public IEnumerable<Claim> GetClaims(string token)
        {
            // 验证和解码 token
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(token)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero  // 可根据需要调整时钟偏移量
            };

            SecurityToken validatedToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return principal.Claims;
        }

        /// <summary>
        /// 私有方法，用于生成Token字符串
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string CreateTokenString(List<Claim> claims, DateTime expires)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtModel.Issuer,
                audience: _jwtModel.Audience,
                claims: claims,           //携带的荷载
                notBefore: DateTime.Now,  //token生成时间
                expires: expires,         //token过期时间
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.SecretKey)), SecurityAlgorithms.HmacSha256
                    )
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
