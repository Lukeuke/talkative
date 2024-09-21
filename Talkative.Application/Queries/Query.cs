using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talkative.Application.Helpers;
using Talkative.Application.Queries.Abstraction;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Queries;

public class Query : IRoomQuery, IMessageQuery
{
    [Authorize]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Room>> GetAllRooms(
        [Service] ApplicationContext context,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new Exception($"User with ID: {userId} was not found.");
        }

        return user.Rooms;
    }

    [Authorize]
    [UseFiltering]
    [UseSorting]
    public IEnumerable<Message> GetAllMessages(Guid groupId)
    {
        return new List<Message>();
    }
}