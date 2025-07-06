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
        public BinaryTreeUserControl() : base()
        {

        }

        public override NodeUserControl? AddNode(string value)
        {
            Console.WriteLine("TreeCanvas: " + TreeCanvas.Children.Count);
            Console.WriteLine("NodeList: " + NodeList.Count);
            foreach (var node in NodeList)
            {
                Console.WriteLine("Node: " + node.Value);
            }
            value = value.Trim().ToLower();

            if (HasValue(value))
            {
                MessageBox.Show($"Node {value} existed !", "Create Node Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var newNode = new BinaryNodeUserControl(value);
            NodeList.Add(newNode);
            Console.WriteLine("BinaryTree AddNode: ");
            Console.WriteLine("- NodeList: " + NodeList.Count);
            foreach (var node in NodeList)
            {
                node.PrintNodeInfo();
            }
            Console.WriteLine("- LineList: " + LineList.Count);
            foreach (var line in LineList)
            {
                line.PrintLineInfo();
            }
            Console.WriteLine("- TreeCanvas.Children: " + TreeCanvas.Children.Count);

            if (Root is null)
            {
                Root = newNode;
                AddNodeInCanvas(newNode);
                return newNode;
            }

            Queue<NodeUserControl> queue = new Queue<NodeUserControl>();
            queue.Enqueue(Root);
            while (queue.Count > 0)
            {
                NodeUserControl current = queue.Dequeue();
                SetStateFromRoot(current, NodeVisualState.Traversal);
                Thread.Sleep(AddNodeTraversalDelay);

                if (current.LeftNode == null)
                {
                    current.LeftNode = newNode;
                    newNode.ParentNode = current; // ✅ Gán parent node
                    AddNodeInCanvas(newNode);
                    return newNode;
                }
                else
                {
                    queue.Enqueue(current.LeftNode);
                }

                if (current.RightNode == null)
                {
                    current.RightNode = newNode;
                    newNode.ParentNode = current; // ✅ Gán parent node
                    AddNodeInCanvas(newNode);
                    return newNode;
                }
                else
                {
                    queue.Enqueue(current.RightNode);
                }
                ResetNodeAndChildState(Root);
            }
            return null;
        }

        public override NodeUserControl? RemoveNode(string value)
        {
            NodeUserControl? targetNode = FindNode(value);
            if (targetNode == null)
            {
                MessageBox.Show($"Node {value} not existed !", "Delete Node Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Or victimNode
            var deepestNode = FindDeepestNode(Root);

            NodeList.Remove(targetNode);
            if (targetNode == Root && NodeList.Count == 0 && Root != null)
            {
                Root.FadeOut();
                Root = null;
                return targetNode;
            }

            if (targetNode == deepestNode)
            {
                ClearRelative(targetNode);
                DeleteNodeInCanvas(targetNode, null);
            }
            else
            {
                SwapRelative(targetNode, deepestNode);
                ClearRelative(targetNode);

                DeleteNodeInCanvas(targetNode, deepestNode);
            }
            Console.WriteLine("Delete Node:");
            Console.WriteLine("Target Node: ");
            targetNode?.Click();
            Console.WriteLine("Victim Node: ");
            deepestNode?.Click();
            return targetNode;
        }
    }
}
