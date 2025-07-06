using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    public class UserAttempt
    {
        public string Quizz { get; set; }
        public float Score { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime StartAt { get; set; }
        public string IsCompleted {  get; set; }
    }
}
