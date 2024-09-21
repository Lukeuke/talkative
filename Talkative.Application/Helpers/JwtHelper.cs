using Microsoft.AspNetCore.Http;

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
}