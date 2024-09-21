using Talkative.Application.Services.Identity;

namespace Talkative.Web.Mounts;

public static class MountServices
{
    public static void AddServices(this IServiceCollection collection)
    {
        collection.AddScoped<IIdentityService, IdentityService>();
    }
}