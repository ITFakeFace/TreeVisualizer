using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    public class Tree
    {
        public int Id { get; set; }
        public string? TreeType { get; set; }
        public string? SerializedData { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public string? Name { get; set; }
    }
}
