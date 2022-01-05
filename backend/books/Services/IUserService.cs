using backend.Dtos;
using backend.Entities;

namespace backend.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserDto createUserDto);
        Task<User> GetUserByIdAsync(Guid id);
        Task<string> Login(string email, string password);
    }
}
