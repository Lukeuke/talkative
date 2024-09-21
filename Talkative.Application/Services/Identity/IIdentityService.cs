using Talkative.Application.DTOs.Identity.SignIn;
using Talkative.Application.DTOs.Identity.SignUp;
using Talkative.Application.Enums;

namespace Talkative.Application.Services.Identity;

public interface IIdentityService
{
    Task<(bool, object)> SignUpAsync(SignUpRequestDto requestDto);
    Task<(EResponseStatusCode, object)> SignInAsync(SignInRequestDto requestDto);
}