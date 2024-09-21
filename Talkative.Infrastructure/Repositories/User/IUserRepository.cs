namespace Talkative.Infrastructure.Repositories.User;

public interface IUserRepository
{
    Task<Domain.Entities.User?> GetUserByEmailAsync(string email);
}