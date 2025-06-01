using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TreeVisualizer.Components.Algorithm.BinaryTree
{
    class BinaryTreeUserControl : TreeUserControl
    {
        HashSet<string> NodeValues = new HashSet<string>();
        public BinaryTreeUserControl()
        {

        }

        public override bool AddNode(string value)
        {
            value = value.Trim().ToLower();
            if (NodeValues.Contains(value))
            {
                MessageBox.Show($"Node {value} existed !", "Create Node Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var newNode = new BinaryTreeNodeUserControl(value);
            this.TreeCanvas.Width += newNode.Width;
            NodeValues.Add(value);
            if (Root is null)
            {
                Root = newNode;
                return true;
            }
            Queue<NodeUserControl> queue = new Queue<NodeUserControl>();
            queue.Enqueue(Root);
            while (queue.Count > 0)
            {
                NodeUserControl current = queue.Dequeue();

                if (current.LeftNode == null)
                {
                    current.LeftNode = newNode;
                    return true;
                }
                else
                {
                    queue.Enqueue(current.LeftNode);
                }

                if (current.RightNode == null)
                {
                    current.RightNode = newNode;
                    return true;
                }
                else
                {
                    queue.Enqueue(current.RightNode);
                }
            }
            NodeValues.Remove(value);
            return false;
        }


    }
}
