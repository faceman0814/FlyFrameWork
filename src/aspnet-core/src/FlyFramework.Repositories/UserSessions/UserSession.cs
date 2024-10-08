using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FlyFramework.UserSessions
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId => FindClaim(UserClaimTypes.UserId)?.Value ?? string.Empty;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName => FindClaim(UserClaimTypes.UserName)?.Value ?? string.Empty;

        public string TenantId => FindClaim(UserClaimTypes.TenantId)?.Value ?? string.Empty;

        public string TenantName => FindClaim(UserClaimTypes.TenantName)?.Value ?? string.Empty;

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsAdmin => UserId == "4957adb8870f4e79882231537ff5d3b9";


        public virtual IEnumerable<string> RoleName => FindClaims(UserClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

        public virtual Claim FindClaim(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims.Where(c => c.Type == claimType).ToArray() ?? new Claim[0];
        }
    }
}
