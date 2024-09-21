using HotChocolate;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talkative.Application.Helpers;
using Talkative.Application.Mutations.Abstraction;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Mutations;

public class Mutation : IRoomMutation
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
}