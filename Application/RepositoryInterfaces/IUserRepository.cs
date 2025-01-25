using MindWaveApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Aplication.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(int id);
        Task<User> GetUserAsync(string email);
        Task CreateUserAsync(User user, string passwordHash);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
