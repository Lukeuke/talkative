using Talkative.Application.Services.Device;
using Talkative.Application.Services.Identity;
using Talkative.Infrastructure.Helpers;
using Talkative.Worker;

namespace Talkative.Web.Mounts;

public static class MountServices
{
    public static void AddServices(this IServiceCollection collection)
    {
        collection.AddScoped<IIdentityService, IdentityService>();
        collection.AddScoped<IMobileDeviceService, MobileDeviceService>();

        collection.AddSingleton<UserStatusHelper>();
        
        collection.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        collection.AddHostedService<Worker.Worker>();
    }
}