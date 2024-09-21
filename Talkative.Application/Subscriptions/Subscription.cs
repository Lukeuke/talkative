using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talkative.Application.Helpers;
using Talkative.Application.Subscriptions.Abstraction;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Subscriptions;

public class Subscription : IMessageSubscription
{
    [Authorize]
    [Subscribe(MessageType = typeof(Message))]
    public async Task<ISourceStream<Message>> MessageCreated(
        Guid roomId,
        [Service] ApplicationContext context,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ITopicEventReceiver eventReceiver)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            Console.WriteLine($"User {userId} not found.");
            throw new Exception($"Couldn't find user with this ID: {userId}");
        }

        var isUserInRoom = user.Rooms.Any(x => x.Id == roomId);
        if (!isUserInRoom)
        {
            Console.WriteLine($"User {userId} is not in room {roomId}");
            throw new Exception("You are not part of this room and cannot subscribe to messages.");
        }
        
        Console.WriteLine($"User {userId} is subscribing to room {roomId}");
        return await eventReceiver.SubscribeAsync<Message>($"MessageCreated_{roomId}");
    }
}