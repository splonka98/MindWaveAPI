using MindWaveCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveCore.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(int id);
        Task<User> GetUserAsync(string email, string password);
        Task CreateUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
