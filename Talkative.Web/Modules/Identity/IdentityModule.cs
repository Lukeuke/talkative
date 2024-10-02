using Microsoft.AspNetCore.Mvc;
using Talkative.Application.DTOs.Identity.SignIn;
using Talkative.Application.DTOs.Identity.SignUp;
using Talkative.Application.DTOs.Other;
using Talkative.Application.Enums;
using Talkative.Application.Helpers;
using Talkative.Application.Services.Device;
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
            [FromServices] IIdentityService identityService,
            [FromServices] IHttpContextAccessor httpContextAccessor
        ) =>
        {
            var headers = httpContextAccessor.HttpContext!.Request.Headers;
            
            headers.TryGetValue("X-Device", out var val);
            headers.TryGetValue("X-ExpoPush-Token", out var expoToken);
            
            var (status, result) = await identityService.SignInAsync(requestDto, val, expoToken);

            return status switch
            {
                EResponseStatusCode.Ok => Results.Ok(result),
                EResponseStatusCode.BadRequest => Results.BadRequest(result),
                EResponseStatusCode.NotFound => Results.NotFound(result),
                _ => Results.BadRequest(result)
            };
        });

        app.MapPost("api/identity/logout", async (
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IMobileDeviceService mobileDeviceService,
            [FromHeader] string authorization
            ) =>
        {
            var headers = httpContextAccessor.HttpContext!.Request.Headers;

            headers.TryGetValue("X-Device", out var val);
            headers.TryGetValue("X-ExpoPush-Token", out var expoToken);

            if (string.IsNullOrEmpty(expoToken))
            {
                return Results.BadRequest(new MessageResponseDto("Expo push token is missing."));
            }
            
            if (val.Equals("Mobile"))
            {
                var userId = authorization.GetUserIdFromJwt();

                await mobileDeviceService.DisassociatePushTokenFromUser(userId, expoToken!);
            }

            return Results.BadRequest(new MessageResponseDto("This endpoint is only accessible for Mobile devices."));

        }).RequireAuthorization();
    }
}