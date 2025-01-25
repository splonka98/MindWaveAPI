using MindWaveApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Aplication.RepositoryInterfaces
{
    public interface IQueryRepository
    {
        Task<Query> GetQueryAsync(int id);
        Task CreateQueryAsync(Query query);
        Task UpdateQueryAsync(Query query);
        Task DeleteQueryAsync(int id);
    }
}