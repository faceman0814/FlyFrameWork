using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.JWTTokens
{
    public interface IJWTTokenManager
    {
        string GenerateToken(List<Claim> claims);
    }
}
