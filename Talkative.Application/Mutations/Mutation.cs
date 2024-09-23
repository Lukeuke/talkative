using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talkative.Application.Helpers;
using Talkative.Application.Mutations.Abstraction;
using Talkative.Domain.Entities;
using Talkative.Domain.Models;
using Talkative.Infrastructure.Context;
using Talkative.Infrastructure.Helpers;
using Talkative.Worker;

namespace Talkative.Application.Mutations;

public class Mutation : IRoomMutation, IMessageMutation
{
    [Authorize]
    public async Task<Room> CreateRoom(
        string name, 
        [Service] ApplicationContext context, 
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = userId,
        };
    
        var user = await context.Users
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new Exception($"User with ID: {userId} was not found.");
        }
        
        user.Rooms.Add(room);
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            await context.Rooms.AddAsync(room);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("An error occurred while creating the room.", ex);
        }

        return room;
    }
    
    [Authorize]
    public async Task<Message> SendMessage(
        Guid roomId,
        string content,
        [Service] ApplicationContext context,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ITopicEventSender sender)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var room = await context.Rooms
            .Include(x => x.Users)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.Id == roomId);

        if (room == null)
        {
            throw new Exception("Room was not found");
        }

        var hasUser = room.Users.Any(x => x.Id == userId);

        if (!hasUser)
        {
            throw new Exception("You are not part of this room and cannot send messages.");
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = content,
            CreatedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
            EditedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
            SenderId = userId,
            RoomId = roomId
        };

        await context.Messages.AddAsync(message);
        await context.SaveChangesAsync();

        // await sender.SendAsync($"MessageCreated_{roomId}", message);
        await sender.SendAsync($"MessageCreated", message);
        Console.WriteLine($"Message sent to room {roomId} by user {userId}");
        
        return message;
    }

    [Authorize]
    public async Task<bool> SetUserStatus(
        bool isOnline,
        [Service] ITopicEventSender eventSender,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IBackgroundTaskQueue taskQueue,
        [Service] UserStatusHelper userStatusHelper
        )
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();
        
        var status = new UserStatus
        {
            UserId = userId,
            IsOnline = isOnline
        };

        taskQueue.EnqueueTask(status);
        await eventSender.SendAsync("UserStatusChanged", status);

        await userStatusHelper.UpdateUserStatus(userId, isOnline);
        
        return true;
    }

    [Authorize]
    public async Task<bool> InviteUser(
        string email, 
        Guid roomId,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context
        )
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var room = await context.Rooms.FirstOrDefaultAsync(x => x.Id == roomId);

        if (room is null)
        {
            return false;
        }

        if (room.OwnerId != userId)
        {
            return false;
        }

        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return false;
        }

        if (context.Invites.Any(x => x.UserId == user.Id && x.RoomId == roomId))
        {
            return false;
        }
        
        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            UserId = user.Id,
            RoomName = room.Name
        };

        await context.Invites.AddAsync(invite);
        await context.SaveChangesAsync();
        // TODO: make subscription and emit it
        return true;
    }

    [Authorize]
    public async Task<Room> AcceptInvite(
        Guid inviteId,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ApplicationContext context)
    {
        var userId = httpContextAccessor.GetUserIdFromJwt();

        var invite = await  context.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);

        if (invite is null)
        {
            throw new Exception($"Couldn't find an invite with ID: {inviteId}");
        }

        if (invite.UserId != userId)
        {
            throw new Exception("You don't have access to this invite.");
        }

        var user = await context.Users.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == userId);
        var room = await context.Rooms.FirstOrDefaultAsync(x => x.Id == invite.RoomId);
        
        if (room == null)
        {
            throw new Exception($"Couldn't find an room with ID: {invite.RoomId}");
        } 
        
        user?.Rooms.Add(room);
        context.Invites.Remove(invite);
        
        await context.SaveChangesAsync();
        return room;
    }
}