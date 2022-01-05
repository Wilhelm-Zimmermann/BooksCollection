using backend.Database;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DatabaseContext _context;

        public UsersRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await  _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
