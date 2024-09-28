using FlyFramework.Dependencys;

using System;
using System.Collections.Generic;
using System.Security.Claims;
namespace FlyFramework.Utilities.JWTTokens
{
    public interface IJWTTokenManager : ISingletonDependency
    {
        string GenerateToken(List<Claim> claims, DateTime expiration);
        IEnumerable<Claim> GetClaims(string token);
    }
}
