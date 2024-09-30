using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Talkative.Domain.Models;

namespace Talkative.Application.Helpers;

public static class JwtHelper
{
    public static Guid GetUserIdFromJwt(this IHttpContextAccessor httpContextAccessor)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];

        if (authorizationHeader is null)
        {
            throw new Exception("Authorization header is missing.");
        }

        var token = authorizationHeader.ToString()?.Split(" ")[^1];

        var userIdClaim = token!.DeAssembleClaimsIdentity("id")?.Value;

        if (userIdClaim is null)
        {
            throw new Exception("Jwt is not valid.");
        }

        return Guid.Parse(userIdClaim);
    }
    
    public static Guid GetUserIdFromJwt(this string authorizationHeader)
    {
        var token = authorizationHeader.Split(" ")[^1];

        var userIdClaim = token!.DeAssembleClaimsIdentity("id")?.Value;

        if (userIdClaim is null)
        {
            throw new Exception("Jwt is not valid.");
        }

        return Guid.Parse(userIdClaim);
    }

    public static bool ValidateJwt(this string token, Settings settings, out JwtSecurityToken? jwt)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey)),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidIssuer = settings.Issuer,
            ValidateIssuer = true,
        };

        var principal = jwtTokenHandler.ValidateToken(token, validationParameters, out _);

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            jwt = (JwtSecurityToken)validatedToken;
    
            return true;
        } 
        catch (SecurityTokenValidationException ex)
        {
            jwt = null;
            return false;
        }
    }
    
    public static bool Validate(this string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            if (tokenS is null) return false;
            
            if (tokenS.ValidFrom > DateTime.Now)
            {
                return false;
            }
            
            if (tokenS.ValidTo < DateTime.Now)
            {
                return false;
            }

            var alg = (string)tokenS.Header.First(x => x.Key == "alg").Value;
            
            if (alg is "" or "None")
            {
                return false;
            }

            SecurityToken validatedToken;
            IPrincipal principal = handler.ValidateToken(token, GetValidationParameters(), out validatedToken);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private static TokenValidationParameters GetValidationParameters()
    {
        const string key = "ADSJKJ219DJ912D29kjdj301d1238j2jd8oj3d32jid3i2dj328032jdj32d80jadhn2819dh21dhu12";
        
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = "Talkative",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) //TODO: Get from service settings
        };
    }
}