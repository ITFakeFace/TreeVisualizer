using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TreeVisualizer.Components.Algorithm.BinaryTree;
using TreeVisualizer.Utils.Coordinator;

namespace TreeVisualizer.Components.Algorithm
{
    /// <summary>
    /// Interaction logic for TreeUserControl.xaml
    /// </summary>
    public partial class TreeUserControl : UserControl
    {
        public List<NodeUserControl> TraversalList = new List<NodeUserControl>();
        public List<NodeUserControl> NodeList = new List<NodeUserControl>();
        public List<ConnectorLine> LineList = new List<ConnectorLine>();
        public NodeUserControl? Root { get; set; } = null;
        public TreeUserControl()
        {
            InitializeComponent();
            Canvas.SetLeft(this, 200);
            Canvas.SetTop(this, 200);
            //this.Width = 1000;
        }

        public virtual void RenderTree()
        {

        }

        public void AddNodeInCanvas(NodeUserControl? node)
        {
            if (node == null) return;

            var size = MainWindow.CoordCalculator.GridSize;
            Console.WriteLine("AddNodeInCanvas: NodeList: " + NodeList.Count);
            Console.WriteLine("AddNodeInCanvas: GridSize: " + size);

            if (double.IsNaN(TreeCanvas.Width))
                TreeCanvas.Width = 0;

            if (double.IsNaN(TreeCanvas.Height))
                TreeCanvas.Height = 0;

            TreeCanvas.Width += size.X;
            TreeCanvas.Height += size.X;
            Console.WriteLine("AddNodeInCanvas: TreeCanvas.Width: " + TreeCanvas.Width);
            Console.WriteLine("AddNodeInCanvas: TreeCanvas.Height: " + TreeCanvas.Height);

            GenerateIndex(Root);

            var CoordCalc = MainWindow.CoordCalculator;
            // Appear
            Coordinate src = CoordCalc.GetNodeCoordinate(Root == null ? 0 : Root.X + 2, 0);
            TreeCanvas.Children.Add(node);
            Canvas.SetLeft(node, src.X);
            Canvas.SetTop(node, src.Y);
            node.FadeIn();
            // Go to Its Coordinate (if it is already in right coordinate => go to but there's no change position)
            foreach (var child in NodeList)
            {
                Coordinate dest = CoordCalc.GetNodeCoordinate(child.X, child.Y);
                child.GoTo(dest.X, dest.Y);
            }

            //Add new ConnectorLine
            ConnectorLine line = new ConnectorLine();
            line.Connect(node.ParentNode, node);
            LineList.Add(line);
            TreeCanvas.Children.Add(line);
            line.AnimateAppear();
            //Update Old ConnectorLine
            foreach (var childLine in LineList)
            {
                childLine.UpdateLine();
            }
        }
        public void DeleteNodeInCanvas(NodeUserControl? targetNode, NodeUserControl? victimNode)
        {
            if (targetNode == null) return;

            // Get canvas size info
            var size = MainWindow.CoordCalculator.GridSize;
            Console.WriteLine("DeleteNodeInCanvas: NodeList.Count = " + NodeList.Count);
            Console.WriteLine("DeleteNodeInCanvas: GridSize = " + size);
            var CoordCalc = MainWindow.CoordCalculator;

            // Animation: Fade Out
            targetNode.FadeOut(() =>
            {
                // 1. Remove targetNode from canvas
                TreeCanvas.Children.Remove(targetNode);

                // 2. Remove node from NodeList
                NodeList.Remove(targetNode);

                // 3. Remove lines connected to targetNode
                var linesToRemove = LineList
                    .Where(line => line.StartElement == targetNode || line.EndElement == targetNode)
                    .ToList();

                foreach (var line in linesToRemove)
                {
                    TreeCanvas.Children.Remove(line);
                    LineList.Remove(line);
                }

                // 4. Update parent references
                if (victimNode != null)
                {
                    victimNode.LeftNode = targetNode.LeftNode == victimNode ? null : targetNode.LeftNode;
                    victimNode.RightNode = targetNode.RightNode == victimNode ? null : targetNode.RightNode;

                    if (victimNode.LeftNode != null)
                        victimNode.LeftNode.ParentNode = victimNode;

                    if (victimNode.RightNode != null)
                        victimNode.RightNode.ParentNode = victimNode;

                    if (targetNode.ParentNode == null)
                    {
                        Root = victimNode;
                        victimNode.ParentNode = null;
                    }
                    else
                    {
                        victimNode.ParentNode = targetNode.ParentNode;

                        if (targetNode.ParentNode.LeftNode == targetNode)
                            targetNode.ParentNode.LeftNode = victimNode;
                        else if (targetNode.ParentNode.RightNode == targetNode)
                            targetNode.ParentNode.RightNode = victimNode;
                    }

                    // 🔗 Reconnect lines
                    if (victimNode.ParentNode != null)
                    {
                        var parentLine = new ConnectorLine();
                        parentLine.Connect(victimNode.ParentNode, victimNode);
                        LineList.Add(parentLine);
                        TreeCanvas.Children.Add(parentLine);
                        parentLine.AnimateAppear();
                    }

                    if (victimNode.LeftNode != null)
                    {
                        var leftLine = new ConnectorLine();
                        leftLine.Connect(victimNode, victimNode.LeftNode);
                        LineList.Add(leftLine);
                        TreeCanvas.Children.Add(leftLine);
                        leftLine.AnimateAppear();
                    }

                    if (victimNode.RightNode != null)
                    {
                        var rightLine = new ConnectorLine();
                        rightLine.Connect(victimNode, victimNode.RightNode);
                        LineList.Add(rightLine);
                        TreeCanvas.Children.Add(rightLine);
                        rightLine.AnimateAppear();
                    }
                }



                // 5. Regenerate coordinates
                GenerateIndex(Root);

                // 6. Update victimNode position (if any)
                if (victimNode != null)
                {
                    // Move victimNode to position of targetNode (coordinate-wise)
                    victimNode.GoTo(Canvas.GetLeft(targetNode), Canvas.GetTop(targetNode));
                }


                var coordCalc = MainWindow.CoordCalculator;
                foreach (var child in NodeList)
                {
                    Coordinate dest = coordCalc.GetNodeCoordinate(child.X, child.Y);
                    child.GoTo(dest.X, dest.Y);
                }

                // 7. Update all lines
                foreach (var line in LineList)
                {
                    line.UpdateLine();
                }

                // 8. Adjust canvas size (if you still want to shrink it)
                TreeCanvas.Width = Math.Max(0, TreeCanvas.Width - size.X);
                TreeCanvas.Height = Math.Max(0, TreeCanvas.Height - size.Y);

                Console.WriteLine("DeleteNodeInCanvas: Done");
            });
        }


        public void GenerateIndex(NodeUserControl? node)
        {
            int currentX = 0;

            void Traverse(NodeUserControl? current, int level)
            {
                if (current == null) return;

                Traverse(current.LeftNode, level + 1);

                current.X = currentX++;
                current.Y = level;

                Traverse(current.RightNode, level + 1);
            }

            Traverse(node, 0);
        }

        public void SetPathFromRoot(NodeUserControl? current, NodeVisualState state)
        {
            while (current != null)
            {
                current.NodeState = state;
                current = current.ParentNode;
            }
        }

        public void ResetNodeAndChildState(NodeUserControl? current)
        {
            if (current == null)
                return;
            current.NodeState = NodeVisualState.Default;
            ResetNodeAndChildState(current.GetLeftNode());
            ResetNodeAndChildState(current.GetRightNode());
        }

        public virtual NodeUserControl? AddNode(string value)
        {
            return null;
        }

        public virtual NodeUserControl? RemoveNode(string value)
        {
            return null;
        }

        public virtual List<string> Traverse(string order)
        {
            var result = new List<string>();
            switch (order.ToUpper())
            {
                case "LNR":
                    TraverseLNR(Root, ref result);
                    break;
                case "LRN":
                    return TraverseLRN(result);
                case "NLR":
                    return TraverseNLR(result);
                case "NRL":
                    return TraverseNRL(result);
                case "RLN":
                    return TraverseRLN(result);
                case "RNL":
                    return TraverseRNL(result);
                default:
                    break;
            }
            Console.WriteLine();
            return result;
        }

        private List<string> TraverseLNR(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            TraverseLNR(node.GetLeftNode(), ref result);
            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            TraverseLNR(node.GetRightNode(), ref result);
            return result;
        }

        private List<string> TraverseLRN(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseNLR(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseNRL(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseRLN(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseRNL(List<string> result)
        {
            return new List<string>();
        }

        public virtual NodeUserControl? FindNode(string value)
        {
            value = value.Trim().ToLower();
            foreach (var node in NodeList)
            {
                if (node.Value.Equals(value))
                    return node;
            }
            return null;
        }
    }
}
