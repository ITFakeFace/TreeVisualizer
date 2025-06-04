using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Utils.Coordinator
{
    public class Coordinate
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public Coordinate()
        {

        }
        public Coordinate(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public override string ToString()
        {
            return $"Coord{{(X:{X},Y:{Y})}}";
        }
    }
}
