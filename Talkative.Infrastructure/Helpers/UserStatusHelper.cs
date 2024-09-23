using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talkative.Infrastructure.Context;

namespace Talkative.Infrastructure.Helpers;

public class UserStatusHelper
{
    private readonly IServiceProvider _serviceProvider;

    public UserStatusHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> UpdateUserStatus(Guid userId, bool isOnline)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            await context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(b =>
                    b.SetProperty(u => u.OnlineStatus, isOnline)
                );
            await context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}