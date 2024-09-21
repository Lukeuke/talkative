using AutoMapper;
using Talkative.Application.DTOs.Identity.SignIn;
using Talkative.Application.DTOs.Identity.SignUp;
using Talkative.Application.DTOs.Other;
using Talkative.Application.Enums;
using Talkative.Application.Helpers;
using Talkative.Domain.Entities;
using Talkative.Domain.Models;
using Talkative.Infrastructure.Repositories.Abstraction;
using Talkative.Infrastructure.Repositories.User;

namespace Talkative.Application.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUserRepository _repository;
    private readonly BaseRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly Settings _settings;

    public IdentityService(
        IUserRepository repository,
        BaseRepository<User> userRepository,
        IMapper mapper,
        Settings settings)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
        _settings = settings;
    }

    public async Task<(bool, object)> SignUpAsync(SignUpRequestDto requestDto)
    {
        var isUser = await _repository.GetUserByEmailAsync(requestDto.Email);

        if (isUser is not null)
        {
            return (false, new MessageResponseDto("Email is already in use."));
        }

        // TODO: Add fluent validation (Email)

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = requestDto.Username,
            FirstName = requestDto.FirstName,
            LastName = requestDto.LastName,
            PasswordHash = requestDto.Password,
            Email = requestDto.Email,
            Salt = "",
            CreatedAt = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        user.ProvideSaltAndHash();

        try
        {
            await _userRepository.AddAsync(user);

            var response = _mapper.Map<SignUpResponseDto>(user);

            return (true, response);
        }
        catch
        {
            return (false, new MessageResponseDto("Failed to create user."));
        }
    }

    public async Task<(EResponseStatusCode, object)> SignInAsync(SignInRequestDto requestDto)
    {
        var user = await _repository.GetUserByEmailAsync(requestDto.Email);

        if (user is null)
        {
            return (EResponseStatusCode.NotFound, new MessageResponseDto("Couldn't find a user with this email."));
        }

        if (user.PasswordHash != AuthenticationHelper.GenerateHash(requestDto.Password, user.Salt))
        {
            return (EResponseStatusCode.BadRequest, new MessageResponseDto("Password is not valid."));
        }

        // var userClaimsModel = _mapper.Map<UserClaimsModel>(user);

        var token = AuthenticationHelper.GenerateJwt(AuthenticationHelper.AssembleClaimsIdentity(user), _settings);

        return (EResponseStatusCode.Ok, new TokenResponseDto(token, _settings.Expiration));
    }
}