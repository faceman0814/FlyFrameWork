using FlyFramework.RoleService;
using FlyFramework.UserService;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyFramework.Authorizations
{
    public class LogInManager : ILogInManager
    {
        protected readonly IUserSession _abpSession;
        protected readonly UserClaimsPrincipalFactory<User, Role> _claimsPrincipalFactory;

        public LogInManager(IUserSession abpSession, UserClaimsPrincipalFactory<User, Role> claimsPrincipalFactory)
        {
            _abpSession = abpSession;
            _claimsPrincipalFactory = claimsPrincipalFactory;
        }

        public async Task<ClaimsIdentity> LoginAsync(User user)
        {
            return (ClaimsIdentity)(await _claimsPrincipalFactory.CreateAsync(user)).Identity;
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
    }
}
