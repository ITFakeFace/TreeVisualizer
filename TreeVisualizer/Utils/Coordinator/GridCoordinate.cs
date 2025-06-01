using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Utils.Coordinator
{
    public class GridCoordinate
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        public GridCoordinate(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public override string ToString()
        {
            return $"(X:{X},Y:{Y})";
        }
    }
}
