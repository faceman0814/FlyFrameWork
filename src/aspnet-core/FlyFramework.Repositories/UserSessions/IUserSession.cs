using FlyFramework.Common.Dependencys;

using System.Security.Claims;

namespace FlyFramework.Repositories.UserSessions
{
    public interface IUserSession : ITransientDependency
    {
        public string UserId { get; }

        public string UserName { get; }

        Claim FindClaim(string claimType);

        Claim[] FindClaims(string claimType);
    }
}
