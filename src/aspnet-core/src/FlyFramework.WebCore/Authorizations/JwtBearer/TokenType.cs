using System.ComponentModel;

namespace FlyFramework.Authorizations.JwtBearer
{
    public enum TokenType
    {
        [Description("Access Token")]
        AccessToken,
        [Description("Refresh Token")]
        RefreshToken
    }
}
