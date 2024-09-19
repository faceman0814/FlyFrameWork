using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Helpers.JWTTokens
{
    public class JWTTokenManager : IJWTTokenManager
    {
        private IJwtBearerModel jwtModel;
        private readonly IConfiguration _configuration;

        public JWTTokenManager(IJwtBearerModel jwtModel, IConfiguration configuration)
        {
            this.jwtModel = jwtModel;
            _configuration = configuration;
            jwtModel = _configuration.GetSection("JwtBearer").Get<JwtBearerModel>();
        }

        public string GenerateToken(List<Claim> claims)
        {
            return CreateTokenString(claims);
        }

        /// <summary>
        /// 私有方法，用于生成Token字符串
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string CreateTokenString(List<Claim> claims)
        {
            //过期时间
            DateTime expires = DateTime.Now.AddMinutes(jwtModel.AccessTokenExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtModel.Issuer,
                audience: jwtModel.Audience,
                claims: claims,           //携带的荷载
                notBefore: DateTime.Now,  //token生成时间
                expires: expires,         //token过期时间
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel.SecretKey)), SecurityAlgorithms.HmacSha256
                    )
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
