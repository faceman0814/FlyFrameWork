using Microsoft.IdentityModel.Tokens;

using System;

namespace FlyFramework.Authorizations.JwtBearer
{
    public class TokenAuthConfiguration : ITokenAuthConfiguration
    {
        public SymmetricSecurityKey SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TimeSpan AccessTokenExpiration { get; set; }

        public TimeSpan RefreshTokenExpiration { get; set; }
    }

    public class ITokenAuthConfiguration
    {
        public SymmetricSecurityKey SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TimeSpan AccessTokenExpiration { get; }

        public TimeSpan RefreshTokenExpiration { get; }
    }
}
