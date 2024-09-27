﻿using FlyFramework.Common.Dependencys;

using System.Security.Claims;

namespace FlyFramework.Common.Utilities.JWTTokens
{
    public interface IJWTTokenManager : ISingletonDependency
    {
        string GenerateToken(List<Claim> claims, DateTime expiration);
        IEnumerable<Claim> GetClaims(string token);
    }
}
