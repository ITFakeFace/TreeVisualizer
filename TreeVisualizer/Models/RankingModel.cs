using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    internal class RankingModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public float Score { get; set; }
        public TimeSpan Time { get; set; }

    }
}
