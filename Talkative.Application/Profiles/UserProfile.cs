using AutoMapper;
using Talkative.Application.DTOs.Identity.SignUp;
using Talkative.Domain.Entities;

namespace Talkative.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, SignUpResponseDto>();
    }
}