using System.Security.Claims;

namespace FlyFramework.UserSessions
{
    public interface IUserSession
    {
        public string UserId { get; }

        public string UserName { get; }

        Claim FindClaim(string claimType);

        Claim[] FindClaims(string claimType);
    }
}
