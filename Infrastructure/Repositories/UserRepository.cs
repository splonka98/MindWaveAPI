using MindWaveApi.Aplication.RepositoryInterfaces;
using MindWaveApi.Core.Models;
using MindWaveApi.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Queries)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Queries)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(User user, string passwordHash)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
