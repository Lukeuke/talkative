using System.Runtime.CompilerServices;
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
using Talkative.Domain.Models;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Subscriptions;

public class Subscription : IMessageSubscription
{
    [Authorize]
#pragma warning disable CS0618
    [SubscribeAndResolve]
#pragma warning restore CS0618
    public async ValueTask<ISourceStream<Message>> MessageCreated(
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
            throw new Exception($"Couldn't find user with this ID: {userId}");
        }

        var isUserInRoom = user.Rooms.Any(x => x.Id == roomId);
        if (!isUserInRoom)
        {
            throw new Exception("You are not part of this room and cannot subscribe to messages.");
        }
        
        Console.WriteLine($"User {userId} is subscribing to room {roomId}");
        return await eventReceiver.SubscribeAsync<Message>($"MessageCreated_{roomId}");
    }
    
    [Authorize]
#pragma warning disable CS0618
    [SubscribeAndResolve]
#pragma warning restore CS0618
    public async ValueTask<ISourceStream<Message>> AllGroupMessages(
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
            throw new Exception($"Couldn't find user with this ID: {userId}");
        }

        var userRoomIds = user.Rooms.Select(r => r.Id).ToHashSet(); // Zmieniamy na HashSet dla szybszego sprawdzania

        if (userRoomIds.Count == 0)
        {
            throw new Exception($"User {userId} is not in any rooms.");
        }
        
        var subscription = await eventReceiver.SubscribeAsync<Message>("MessageCreated");

        var filteredStream = new FilteredSourceStream<Message>(subscription, message => userRoomIds.Contains(message.RoomId));

        return filteredStream;
    }
    
    [Authorize]
#pragma warning disable CS0618
    [SubscribeAndResolve] // TODO: this method is obsolete but its the only way i found to subscribe to dynamic topics
#pragma warning restore CS0618
    // https://swacblooms.com/dynamic-subscriptions-in-hot-chocolate/ - this is the method im using
    // https://youtu.be/wHC9gOk__y0?t=436 - this is the method i tried to use dynamic [Topic] as parameter for roomId but
    // it seems like its not working for me cuz the [Topic] is method only. THIS IS THE CORRECT WAY OF DOING IT.
    public async ValueTask<ISourceStream<UserStatus>> OnUserStatusChanged(
        string roomId, 
        /*[EventMessage] UserStatus status,*/ 
        [Service] ITopicEventReceiver receiver,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();
        
        var room = await context.Rooms
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(roomId));

        if (room == null)
        {
            throw new Exception("Room was not found.");
        }
        
        // if user sends status to room that he is not in.
        if (room.Users.All(x => x.Id != userId))
        {
            throw new Exception("You are not in this room.");
        }
        
        return await receiver.SubscribeAsync<UserStatus>(roomId);

        /*if (roomId != status.RoomId)
        {
            return null;
        }*/
        
        // return status;
    }
}

public class FilteredSourceStream<T> : ISourceStream<T>
{
    private readonly ISourceStream<T> _innerStream;
    private readonly Func<T, bool> _filter;

    public FilteredSourceStream(ISourceStream<T> innerStream, Func<T, bool> filter)
    {
        _innerStream = innerStream;
        _filter = filter;
    }

    async IAsyncEnumerable<T> ISourceStream<T>.ReadEventsAsync()
    {
        await foreach (var item in _innerStream.ReadEventsAsync())
        {
            if (_filter(item))
            {
                yield return item;
            }
        }
    }

    public ValueTask DisposeAsync() => _innerStream.DisposeAsync();

    async IAsyncEnumerable<object> ISourceStream.ReadEventsAsync()
    {
        await foreach (var item in _innerStream.ReadEventsAsync())
        {
            if (_filter(item))
            {
                yield return item;
            }
        }
    }
}