using MindWaveApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Domain.Interfaces.ServiceInterfaces
{
    public interface IQueryService
    {
        Task<Query> GetQueryAsync();
        Task<Question> GetNextQuestionAsync(int queryId, int currentQuestionId, List<Answer> answers);
        Task SubmitQueryAsync(int queryId, List<Answer> answers);
    }
}
