using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    public class QuizzDetails
    {
        public int Id { get; set; }
        public int QuizzId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer1 { get; set; } = string.Empty;
        public string Answer2 { get; set; } = string.Empty;
        public string Answer3 { get; set; } = string.Empty;
        public string Answer4 { get; set; } = string.Empty;
        public short CorrectAnswer { get; set; }
    }
}
