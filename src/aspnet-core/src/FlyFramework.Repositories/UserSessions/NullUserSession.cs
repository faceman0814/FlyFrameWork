using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.UserSessions
{
    public class NullUserSession : IUserSession
    {
        //
        // 摘要:
        //     Singleton instance.
        public static NullUserSession Instance { get; } = new NullUserSession();

        public string UserId => null;
        public string UserName => null;
        public string TenantId => null;
        public string TenantName => null;

        public Claim FindClaim(string claimType)
        {
            return ((IUserSession)Instance).FindClaim(claimType);
        }

        public Claim[] FindClaims(string claimType)
        {
            return ((IUserSession)Instance).FindClaims(claimType);
        }
    }
}
