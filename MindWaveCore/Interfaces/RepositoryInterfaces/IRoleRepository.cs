using MindWaveCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveCore.Interfaces.RepositoryInterfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleAsync(int id);
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(int id);
    }
}
