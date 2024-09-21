using Talkative.Domain.Entities;
using Talkative.Infrastructure.Repositories.Abstraction;
using Talkative.Infrastructure.Repositories.User;

namespace Talkative.Web.Mounts;

public static class MountRepositories
{
    public static void AddRepositories(this IServiceCollection collection)
    {
        collection.AddScoped<BaseRepository<User>, UserRepository>();
        collection.AddScoped<IUserRepository, UserRepository>();
    }
}