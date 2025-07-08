using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Components.Algorithm.AVLTree
{
    class AVLTreeUserControl : TreeUserControl
    {
        public new AVLNodeUserControl? Root
        {
            get => base.Root as AVLNodeUserControl;
            set => base.Root = value;
        }

        public override NodeUserControl? AddNode(string value)
        {
            if (!double.TryParse(value, out double val))
                return null;
            Console.WriteLine("TreeCanvas: " + TreeCanvas.Children.Count);
            Console.WriteLine("NodeList: " + NodeList.Count);
            var newNode = new AVLNodeUserControl(value);
            Root = AddNodeRecursive((AVLNodeUserControl?)Root, newNode);
            NodeList.Add(newNode);

            AddNodeInCanvas(newNode);
            ValidateAndFixTreeUI();
            return newNode;
        }

        private AVLNodeUserControl? AddNodeRecursive(AVLNodeUserControl? node, AVLNodeUserControl newNode)
        {
            if (node == null)
                return newNode;

            if (double.Parse(newNode.Value) < double.Parse(node.Value))
            {
                node.LeftNode = AddNodeRecursive((AVLNodeUserControl?)node.LeftNode, newNode);
                if (node.LeftNode != null) node.LeftNode.ParentNode = node;
            }
            else if (double.Parse(newNode.Value) > double.Parse(node.Value))
            {
                node.RightNode = AddNodeRecursive((AVLNodeUserControl?)node.RightNode, newNode);
                if (node.RightNode != null) node.RightNode.ParentNode = node;
            }
            else
            {
                // Duplicate values not allowed
                return node;
            }

            node.UpdateHeight();
            return Rebalance(node);
        }

        public override NodeUserControl? RemoveNode(string value)
        {
            AVLNodeUserControl? victim = null;
            var target = FindNode(value) as AVLNodeUserControl;

            if (target == null)
                return null;
            NodeList.Remove(target);
            Root = RemoveNodeRecursive((AVLNodeUserControl?)Root, value, ref victim);

            //if (victim != null)
            //    DeleteNodeInCanvas(target, victim);

            ValidateAndFixTreeUI();
            return target;
        }


        private AVLNodeUserControl? RemoveNodeRecursive(AVLNodeUserControl? node, string value, ref AVLNodeUserControl? victim)
        {
            if (node == null || !double.TryParse(value, out double key) || !double.TryParse(node.Value, out double currentVal))
                return node;

            if (key < currentVal)
            {
                node.LeftNode = RemoveNodeRecursive((AVLNodeUserControl?)node.LeftNode, value, ref victim);
                if (node.LeftNode != null) node.LeftNode.ParentNode = node;
            }
            else if (key > currentVal)
            {
                node.RightNode = RemoveNodeRecursive((AVLNodeUserControl?)node.RightNode, value, ref victim);
                if (node.RightNode != null) node.RightNode.ParentNode = node;
            }
            else
            {
                // Tìm thấy node cần xóa
                victim = node;

                if (node.LeftNode == null || node.RightNode == null)
                {
                    var child = (AVLNodeUserControl?)(node.LeftNode ?? node.RightNode);
                    if (child != null)
                        child.ParentNode = node.ParentNode;

                    return child;
                }
                else
                {
                    // Có đủ 2 con → tìm node nhỏ nhất cây phải
                    var successor = FindMin((AVLNodeUserControl)node.RightNode!);
                    if (successor != null)
                    {
                        victim = successor;
                        node.Value = successor.Value;
                        node.RightNode = RemoveNodeRecursive((AVLNodeUserControl?)node.RightNode, successor.Value, ref victim);
                        if (node.RightNode != null)
                            node.RightNode.ParentNode = node;
                    }
                }
            }

            node.UpdateHeight();
            return Rebalance(node);
        }


        private AVLNodeUserControl Rebalance(AVLNodeUserControl node)
        {
            int balance = node.GetBalance();

            // Left heavy
            if (balance > 1)
            {
                if (((AVLNodeUserControl?)node.LeftNode)?.GetBalance() < 0)
                {
                    node.LeftNode = RotateLeft((AVLNodeUserControl)node.LeftNode!);
                    if (node.LeftNode != null) node.LeftNode.ParentNode = node;
                }
                var rotated = RotateRight(node);
                ValidateAndFixTreeUI();
                return rotated;
            }

            // Right heavy
            if (balance < -1)
            {
                if (((AVLNodeUserControl?)node.RightNode)?.GetBalance() > 0)
                {
                    node.RightNode = RotateRight((AVLNodeUserControl)node.RightNode!);
                    if (node.RightNode != null) node.RightNode.ParentNode = node;
                }
                var rotated = RotateLeft(node);
                ValidateAndFixTreeUI();
                return rotated;
            }

            return node;
        }


        private AVLNodeUserControl RotateLeft(AVLNodeUserControl x)
        {
            var y = (AVLNodeUserControl)x.RightNode!;
            var T2 = y.LeftNode;

            y.LeftNode = x;
            x.RightNode = T2;

            if (T2 != null) T2.ParentNode = x;

            y.ParentNode = x.ParentNode;
            x.ParentNode = y;

            x.UpdateHeight();
            y.UpdateHeight();

            return y;
        }

        private AVLNodeUserControl RotateRight(AVLNodeUserControl y)
        {
            var x = (AVLNodeUserControl)y.LeftNode!;
            var T2 = x.RightNode;

            x.RightNode = y;
            y.LeftNode = T2;

            if (T2 != null) T2.ParentNode = y;

            x.ParentNode = y.ParentNode;
            y.ParentNode = x;

            y.UpdateHeight();
            x.UpdateHeight();

            return x;
        }

        private AVLNodeUserControl FindMin(AVLNodeUserControl node)
        {
            while (node.LeftNode != null)
                node = (AVLNodeUserControl)node.LeftNode;
            return node;
        }
    }
}
