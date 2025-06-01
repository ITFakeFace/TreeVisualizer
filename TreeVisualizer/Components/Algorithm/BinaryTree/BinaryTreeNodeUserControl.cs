using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Components.Algorithm.BinaryTree
{
    class BinaryTreeNodeUserControl : NodeUserControl
    {
        public BinaryTreeNodeUserControl(string value)
        {
            this.Value = value;
            this.X = 0;
            this.Y = 0;
        }
    }
}
