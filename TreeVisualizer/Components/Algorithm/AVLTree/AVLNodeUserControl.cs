using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Components.Algorithm.AVLTree
{
    class AVLNodeUserControl : NodeUserControl
    {
        public int Height { get; set; } = 1;

        public AVLNodeUserControl(string value) : base()
        {
            this.Value = value;
            this.X = 0;
            this.Y = 0;
        }
        public int GetBalance()
        {
            int leftHeight = (LeftNode as AVLNodeUserControl)?.Height ?? 0;
            int rightHeight = (RightNode as AVLNodeUserControl)?.Height ?? 0;
            return leftHeight - rightHeight;
        }

        public void UpdateHeight()
        {
            int leftHeight = (LeftNode as AVLNodeUserControl)?.Height ?? 0;
            int rightHeight = (RightNode as AVLNodeUserControl)?.Height ?? 0;
            Height = Math.Max(leftHeight, rightHeight) + 1;
        }
    }
}
