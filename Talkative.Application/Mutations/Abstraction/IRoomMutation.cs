using Microsoft.AspNetCore.Http;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Mutations.Abstraction;

public interface IRoomMutation
{
    Task<Room> CreateRoom(string name, ApplicationContext context, IHttpContextAccessor httpContextAccessor);
}