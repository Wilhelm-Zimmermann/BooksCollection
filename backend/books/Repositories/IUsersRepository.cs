using backend.Entities;

namespace backend.Repositories
{
    public interface IUsersRepository
    {
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
