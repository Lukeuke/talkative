using HotChocolate.Authorization;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
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

        var userRoomIds = user.Rooms.Select(r => r.Id).ToHashSet();

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
        
        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new Exception($"Couldn't find user with this ID: {userId}");
        }

        var room = user.Rooms.FirstOrDefault(x => x.Id == Guid.Parse(roomId));
        
        if (room == null)
        {
            throw new Exception($"Couldn't find room with this ID: {roomId}");
        }
        
        var subscription = await receiver.SubscribeAsync<UserStatus>("UserStatusChanged");
        
        var filteredStream = new FilteredSourceStream<UserStatus>(subscription, (status) =>
        {
            var userInRoom = context.Users
                .Where(u => u.Id == status.UserId)
                .Any(u => u.Rooms.Any(r => r.Id == Guid.Parse(roomId)));
        
            return userInRoom;
        });

        return filteredStream;
    }

    private Dictionary<Guid, DateTime> ConnectedUsers { get; set; } = new();
    
    [Authorize]
#pragma warning disable CS0618
    [SubscribeAndResolve]
#pragma warning restore CS0618
    public async ValueTask<ISourceStream<RoomStatus>> RoomStatus(
        Guid roomId,
        [Service] ITopicEventReceiver receiver,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context,
        [Service] ITopicEventSender eventSender
        )
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();
        
        var room = await context.Rooms
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == roomId);

        if (room == null)
        {
            throw new Exception("Room was not found.");
        }
        
        // if user sends status to room that he is not in.
        if (room.Users.All(x => x.Id != userId))
        {
            throw new Exception("You are not in this room.");
        }
        
        /*if (!ConnectedUsers.ContainsKey(userId))
        {
            ConnectedUsers[userId] = DateTime.UtcNow;
        }*/
        
        var stream = await receiver.SubscribeAsync<RoomStatus>($"RoomStatus_{roomId}");
        
        // _ = MonitorSubscriptionAsync(stream, eventSender, context, userId, roomId);

        return stream;
    }
    
    private async Task MonitorSubscriptionAsync(
        ISourceStream stream, 
        ITopicEventSender eventSender,
        ApplicationContext context,
        Guid userId, 
        Guid roomId)
    {
        try
        {
            // Mark user as online
            await UpdateUserStatusAsync(eventSender, context, userId, roomId, true);

            var userIsActive = true;
            var timeout = TimeSpan.FromSeconds(10);
            var lastActivity = DateTime.UtcNow;

            // Start a background task to monitor the user's activity
            var timeoutTask = Task.Run(async () =>
            {
                while (userIsActive)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)); // Check every 30 seconds
                    
                    if (DateTime.UtcNow - lastActivity > timeout)
                    {
                        userIsActive = false;
                        await UpdateUserStatusAsync(eventSender, context, userId, roomId, false);
                        Console.WriteLine($"User {userId} timed out.");
                        break;
                    }
                }
            });

            // Listen to the WebSocket stream
            await foreach (var _ in stream.ReadEventsAsync())
            {
                // Each time we get a message, reset the activity timer
                lastActivity = DateTime.UtcNow;
            }

            // Ensure the timeout task finishes when the stream ends
            await timeoutTask;
        }
        catch (Exception ex)
        {
            await UpdateUserStatusAsync(eventSender, context, userId, roomId, false);
            Console.WriteLine($"Subskrypcja dla użytkownika {userId} zakończyła się z błędem: {ex.Message}");
        }
        finally
        {
            await UpdateUserStatusAsync(eventSender, context, userId, roomId, false);
            Console.WriteLine($"Użytkownik {userId} został usunięty z listy po rozłączeniu.");
        }
    }
    
    private async Task UpdateUserStatusAsync(
        ITopicEventSender eventSender, 
        ApplicationContext context, 
        Guid userId, 
        Guid roomId, 
        bool isOnline)
    {
        if (isOnline)
        {
            ConnectedUsers[userId] = DateTime.UtcNow;
        }
        else
        {
            ConnectedUsers.Remove(userId);
        }

        var status = new UserStatus
        {
            UserId = userId,
            IsOnline = isOnline
        };

        await eventSender.SendAsync(roomId.ToString(), status);

        await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(b => 
                b.SetProperty(u => u.OnlineStatus, isOnline)
            );

        await context.SaveChangesAsync();
    }
    
    /*[Subscribe]
    [Topic("OnMessageReceived")]
    public async ValueTask<ISourceStream<Message>> OnMessageReceived(
        [EventMessage] Message message,
        [Service] ITopicEventReceiver receiver,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context
        )
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new Exception($"Couldn't find user with this ID: {userId}");
        }

        var userRoomIds = user.Rooms.Select(r => r.Id).ToHashSet();

        if (userRoomIds.Count == 0)
        {
            throw new Exception($"User {userId} is not in any rooms.");
        }
        
        var subscription = await receiver.SubscribeAsync<Message>("OnMessageReceived");

        var filteredStream = new FilteredSourceStream<Message>(subscription, message => userRoomIds.Contains(message.RoomId));

        return filteredStream;
    }*/

    /// <summary>
    /// This method is only used for Web Socket connection for mobile
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <param name="settings"></param>
    /// <param name="context"></param>
    /// <returns>message</returns>
    /// <exception cref="Exception"></exception>
    // [Authorize]
    [Subscribe]
    [Topic("OnMessageReceived")]
    // NOTE: This should have Authorized header but somehow on client-side the Authorization header is being sent but on server it's not available :/
    // Any custom sent header is not.
    public async Task<Message?> OnMessageReceived(
        [EventMessage] Message message,
        string token, 
        [Service] Settings settings,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context
        ) 
    {
        if (!token.ValidateJwt(settings, out _))
        {
            throw new GraphQLException("Not authorized");
        }
        
        var userId = token.GetUserIdFromJwt();
        
        // var userId = httpContextAccessor.GetUserIdFromJwt();
        
        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new GraphQLException($"Couldn't find user with this ID: {userId}");
        }

        var userRoomIds = user.Rooms.Select(r => r.Id).ToHashSet();

        if (userRoomIds.Count == 0)
        {
            throw new GraphQLException($"User {userId} is not in any rooms.");
        }
        
        if (!userRoomIds.Contains(message.RoomId))
        {
            return null;
        }

        return message;
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