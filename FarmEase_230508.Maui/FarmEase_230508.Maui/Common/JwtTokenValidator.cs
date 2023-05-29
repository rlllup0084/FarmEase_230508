using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Common;

public static class JwtTokenValidator
{
    public static bool IsTokenValid(string token) {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token);

        // Check if the token has expired
        var expirationTime = jwtToken.ValidTo;
        var currentTime = DateTime.UtcNow;
        return expirationTime > currentTime;
    }
}
