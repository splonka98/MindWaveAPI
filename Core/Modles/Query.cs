using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Domain.Models
{
    public class Query
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
