using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeVisualizer.Components.Algorithm.BinaryTree;

namespace TreeVisualizer.Components.Algorithm.BinarySearchTree
{
    class BinarySearchTreeUserControl : TreeUserControl
    {
        public BinarySearchTreeUserControl() : base()
        {

        }

        public override NodeUserControl? AddNode(string value)
        {
            if (!int.TryParse(value, out int intValue))
            {
                MessageBox.Show("Invalid input. Only numeric values are allowed.");
                return null;
            }

            Console.WriteLine("TreeCanvas: " + TreeCanvas.Children.Count);
            Console.WriteLine("NodeList: " + NodeList.Count);

            BinarySearchNodeUserControl newNode = new BinarySearchNodeUserControl(value);

            if (Root == null)
            {
                Root = newNode;
                NodeList.Add(newNode);
                AddNodeInCanvas(newNode);
                return Root;
            }

            var current = Root as BinarySearchNodeUserControl;

            while (current != null)
            {
                current.NodeState = NodeVisualState.Traversal;
                Thread.Sleep(AddNodeTraversalDelay);

                if (intValue < int.Parse(current.Value))
                {
                    if (current.LeftNode == null)
                    {
                        current.LeftNode = newNode;
                        newNode.ParentNode = current; // ✅ Gán quan hệ cha
                        break;
                    }
                    current = current.LeftNode as BinarySearchNodeUserControl;
                }
                else if (intValue > int.Parse(current.Value))
                {
                    if (current.RightNode == null)
                    {
                        current.RightNode = newNode;
                        newNode.ParentNode = current; // ✅ Gán quan hệ cha
                        break;
                    }
                    current = current.RightNode as BinarySearchNodeUserControl;
                }
                else
                {
                    MessageBox.Show("Duplicated value. Node already exists.");
                    return null;
                }
            }

            ResetNodeAndChildState(Root);
            NodeList.Add(newNode);
            AddNodeInCanvas(newNode);
            return newNode;
        }


        public override NodeUserControl? RemoveNode(string value)
        {
            NodeUserControl? current = FindNode(value);
            if (current == null)
                return null;

            Root = RemoveNodeRecursive(Root, value);

            ValidateAndFixTreeUI();
            ResetNodeAndChildState(Root);
            return current;
        }

        public NodeUserControl? RemoveNodeRecursive(NodeUserControl? current, string value)
        {
            if (current == null || !double.TryParse(value, out double key) || !double.TryParse(current.Value, out double currentVal))
                return current;

            if (key < currentVal)
            {
                SetStateToChild(current.RightNode, NodeVisualState.Disable);
                current.LeftNode = RemoveNodeRecursive(current.LeftNode, value);
                if (current.LeftNode != null)
                    current.LeftNode.ParentNode = current;
            }
            else if (key > currentVal)
            {
                SetStateToChild(current.LeftNode, NodeVisualState.Disable);
                current.RightNode = RemoveNodeRecursive(current.RightNode, value);
                if (current.RightNode != null)
                    current.RightNode.ParentNode = current;
            }
            else
            {
                current.NodeState = NodeVisualState.Delete;
                // Node cần xóa được tìm thấy
                // Case 1: Node không có con
                if (current.LeftNode == null && current.RightNode == null)
                {
                    if (current.ParentNode != null)
                    {
                        if (current.ParentNode.LeftNode == current)
                            current.ParentNode.LeftNode = null;
                        else if (current.ParentNode.RightNode == current)
                            current.ParentNode.RightNode = null;
                    }
                    NodeList.Remove(current);
                    return null;
                }

                // Case 2: Node chỉ có 1 con
                if (current.LeftNode == null || current.RightNode == null)
                {
                    var child = current.LeftNode ?? current.RightNode;
                    if (child != null)
                        child.ParentNode = current.ParentNode;

                    if (current.ParentNode != null)
                    {
                        if (current.ParentNode.LeftNode == current)
                            current.ParentNode.LeftNode = child;
                        else if (current.ParentNode.RightNode == current)
                            current.ParentNode.RightNode = child;
                    }

                    NodeList.Remove(current);
                    return child;
                }

                // Case 3: Node có cả 2 con → tìm node nhỏ nhất ở cây con phải
                var successor = FindMin(current.RightNode);
                if (successor != null)
                {
                    current.Value = successor.Value; // Sao chép giá trị
                    current.RightNode = RemoveNodeRecursive(current.RightNode, successor.Value);
                    if (current.RightNode != null)
                        current.RightNode.ParentNode = current;
                }
            }

            return current;
        }


        private NodeUserControl FindMin(NodeUserControl? node)
        {
            while (node.LeftNode != null)
            {
                node = node.LeftNode;
            }
            return node!;
        }
    }
}
