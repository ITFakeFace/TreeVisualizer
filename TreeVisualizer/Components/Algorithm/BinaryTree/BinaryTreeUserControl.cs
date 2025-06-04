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
        public int AddNodeTraversalDelay { get; set; } = 300;
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

            if (NodeValues.Contains(value))
            {
                MessageBox.Show($"Node {value} existed !", "Create Node Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var newNode = new BinaryTreeNodeUserControl(value);
            NodeList.Add(newNode);
            NodeValues.Add(value);

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
                SetPathFromRoot(current, NodeVisualState.Traversal);
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

            NodeValues.Remove(value);
            return null;
        }

        public override NodeUserControl? RemoveNode(string value)
        {
            NodeUserControl? targetNode = FindNode(value);
            if (targetNode == null)
            {
                MessageBox.Show($"Node {value} not existed !", "Delete Node Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            var deepestNode = FindDeepestNode(targetNode);
            if (targetNode == deepestNode)
            {
                DeleteNodeInCanvas(targetNode, null);
            }
            else
            {
                DeleteNodeInCanvas(targetNode, deepestNode);
            }
            NodeValues.Remove(value);
            return targetNode;
        }

        public NodeUserControl? FindDeepestNode(NodeUserControl? node)
        {
            if (node == null)
                return null;

            Queue<NodeUserControl> queue = new();
            queue.Enqueue(node);

            NodeUserControl? deepest = null;

            while (queue.Count > 0)
            {
                deepest = queue.Dequeue();

                if (deepest.LeftNode != null)
                    queue.Enqueue(deepest.LeftNode);

                if (deepest.RightNode != null)
                    queue.Enqueue(deepest.RightNode);
            }

            return deepest;
        }
    }
}
