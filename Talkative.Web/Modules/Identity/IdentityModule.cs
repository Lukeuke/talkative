using Microsoft.AspNetCore.Mvc;
using Talkative.Application.DTOs.Identity.SignIn;
using Talkative.Application.DTOs.Identity.SignUp;
using Talkative.Application.Enums;
using Talkative.Application.Services.Identity;

namespace Talkative.Web.Modules.Identity;

public static class IdentityModule
{
    public static void AddIdentityEndpoint(this WebApplication app)
    {
        app.MapGet("api/identity", () => Results.Ok())
            .RequireAuthorization();
        
        app.MapPut("api/identity", async (
            [FromBody] SignUpRequestDto requestDto,
            [FromServices] IIdentityService identityService
        ) =>
        {
            var (status, result) = await identityService.SignUpAsync(requestDto);

            return status ? Results.Created("", result) : Results.BadRequest(result);
        });
        
        app.MapPost("api/identity", async (
            [FromBody] SignInRequestDto requestDto,
            [FromServices] IIdentityService identityService
        ) =>
        {
            var (status, result) = await identityService.SignInAsync(requestDto);

            return status switch
            {
                EResponseStatusCode.Ok => Results.Ok(result),
                EResponseStatusCode.BadRequest => Results.BadRequest(result),
                EResponseStatusCode.NotFound => Results.NotFound(result),
                _ => Results.BadRequest(result)
            };
        });
    }
}