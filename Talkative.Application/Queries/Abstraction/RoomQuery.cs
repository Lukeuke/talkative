using Microsoft.AspNetCore.Http;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Queries.Abstraction;

public interface IRoomQuery
{
    public Task<IEnumerable<Room>> GetAllRooms(ApplicationContext context, IHttpContextAccessor httpContextAccessor);
}