using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveCore.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Question Question { get; set; }
        public Query Query { get; set; }
    }
}
