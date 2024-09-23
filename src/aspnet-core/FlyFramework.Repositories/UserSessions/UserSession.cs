using FlyFramework.Common.Dependencys;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Repositories.UserSessions
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
        public virtual string UserId => FindClaim(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        /// <summary>
        /// 用户名称
        /// </summary>
        public virtual string UserName => FindClaim(ClaimTypes.Name)?.Value ?? string.Empty;

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsAdmin => UserId == "4957adb8870f4e79882231537ff5d3b9";

        //public virtual IEnumerable<string> RoleName => FindClaims(LoginClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

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
