using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    public class AttemptResult
    {
        public string UserName { get; set; }
        public int CorrectNumber { get; set; }
        public DateTime Time { get; set; }
        public DateTime StartAt { get; set; }
        public string Title { get; set; }
    }
}
