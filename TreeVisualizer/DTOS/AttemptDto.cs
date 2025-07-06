using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.DTOS
{
    internal class AttemptDto
    {
        public int CorrectNumber { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime StartAt { get; set; }
        public string QuizzTitle { get; set; } = string.Empty; // Initialize to avoid null warnings
        public int TotalNumber { get; set; }
        public bool Complete { get; set; }
    }
}
