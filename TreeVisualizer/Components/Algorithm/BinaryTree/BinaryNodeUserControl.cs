using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Components.Algorithm.BinaryTree
{
    class BinaryNodeUserControl : NodeUserControl
    {
        public BinaryNodeUserControl(string value) : base()
        {
            this.Value = value;
            this.X = 0;
            this.Y = 0;
        }
    }
}
