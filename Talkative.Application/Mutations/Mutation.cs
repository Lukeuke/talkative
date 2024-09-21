using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talkative.Application.Helpers;
using Talkative.Application.Mutations.Abstraction;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

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

        await sender.SendAsync($"MessageCreated_{roomId}", message);
        Console.WriteLine($"Message sent to room {roomId} by user {userId}");
        
        return message;
    }

}