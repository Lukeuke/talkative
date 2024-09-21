using Microsoft.EntityFrameworkCore;
using Talkative.Infrastructure.Context;
using Talkative.Infrastructure.Repositories.Abstraction;

namespace Talkative.Infrastructure.Repositories.User;

public class UserRepository : BaseRepository<Domain.Entities.User>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.User?> GetUserByEmailAsync(string email)
    {
        return await Context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
}