using Microsoft.EntityFrameworkCore;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Services.Device;

public class MobileDeviceService : IMobileDeviceService
{
    private readonly ApplicationContext _context;

    public MobileDeviceService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task AssociatePushTokenWithUser(Guid userId, string expoPushToken)
    {
        var existingDevice = await _context.Devices
            .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken && d.UserId != userId);

        if (existingDevice != null)
        {
            _context.Devices.Remove(existingDevice);
        }

        var userDevice = await _context.Devices
            .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken && d.UserId == userId);

        if (userDevice == null)
        {
            userDevice = new UserDevice
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpoPushToken = expoPushToken,
                TokenUpdatedAt = DateTime.Now
            };
            await _context.Devices.AddAsync(userDevice);
        }
        else
        {
            userDevice.TokenUpdatedAt = DateTime.Now;
            _context.Devices.Update(userDevice);
        }

        await _context.SaveChangesAsync();
    }
    
    public async Task DisassociatePushTokenFromUser(Guid userId, string expoPushToken)
    {
        var userDevice = await _context.Devices
            .FirstOrDefaultAsync(d => d.ExpoPushToken == expoPushToken && d.UserId == userId);

        if (userDevice != null)
        {
            _context.Devices.Remove(userDevice);
            await _context.SaveChangesAsync();
        }
    }

}