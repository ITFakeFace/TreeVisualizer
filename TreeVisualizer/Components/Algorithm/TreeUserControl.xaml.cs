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
using System.Windows.Media.Animation;
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
        public int AddNodeTraversalDelay { get; set; } = 300;

        public List<NodeUserControl> TraversalList = new List<NodeUserControl>();
        public List<NodeUserControl> NodeList = new List<NodeUserControl>();
        public List<ConnectorLine> LineList = new List<ConnectorLine>();
        public NodeUserControl? Root { get; set; } = null;
        public TreeUserControl()
        {
            InitializeComponent();
        }

        public virtual void RenderTree()
        {

        }

        public async void AddNodeInCanvas(NodeUserControl? node)
        {
            if (node == null) return;

            var CoordCalc = MainWindow.CoordCalculator;
            var size = MainWindow.CoordCalculator.GridSize;
            Console.WriteLine("AddNodeInCanvas: NodeList: " + NodeList.Count);
            Console.WriteLine("AddNodeInCanvas: GridSize: " + size);

            GenerateIndex(Root);

            if (double.IsNaN(TreeCanvas.Width))
                TreeCanvas.Width = 0;

            if (double.IsNaN(TreeCanvas.Height))
                TreeCanvas.Height = 0;

            var maxX = GetMaxX();
            var maxY = GetMaxY();
            var updatedSize = CoordCalc.GetNodeCoordinate(maxX + 1, maxY + 1);
            TreeCanvas.Width = updatedSize.X;
            TreeCanvas.Height = updatedSize.Y;
            Console.WriteLine("AddNodeInCanvas: TreeCanvas.Width: " + TreeCanvas.Width);
            Console.WriteLine("AddNodeInCanvas: TreeCanvas.Height: " + TreeCanvas.Height);


            // Appear
            Coordinate src = CoordCalc.GetNodeCoordinate(Root == null || NodeList.Count == 1 ? 0 : Root.X + 2, 0);
            TreeCanvas.Children.Add(node);
            Canvas.SetLeft(node, src.X);
            Canvas.SetTop(node, src.Y);
            await node.FadeIn();
            Thread.Sleep(AddNodeTraversalDelay);
            // Go to Its Coordinate (if it is already in right coordinate => go to but there's no change position)
            foreach (var child in NodeList)
            {
                Coordinate dest = CoordCalc.GetNodeCoordinate(child.X, child.Y);
                child.GoTo(dest.X, dest.Y);
            }

            //Add new ConnectorLine
            if (node.ParentNode != null)
            {
                ConnectorLine line = new ConnectorLine();
                line.Connect(node.ParentNode, node);
                LineList.Add(line);
                TreeCanvas.Children.Add(line);
                line.AnimateAppear();
            }
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
                // 3.1 Remove lines between victim & its parent if parent not null
                if (victimNode != null && victimNode.Parent != null)
                    linesToRemove.AddRange(LineList.Where(line => line.StartElement == victimNode.Parent || line.EndElement == victimNode).ToList());

                foreach (var line in linesToRemove)
                {
                    TreeCanvas.Children.Remove(line);
                    LineList.Remove(line);
                }

                // 4. Visualize Swap Position if victim not null
                if (victimNode != null)
                {
                    var targetCoord = CoordCalc.GetNodeCoordinate(targetNode.X, targetNode.Y);
                    var victimCoord = CoordCalc.GetNodeCoordinate(victimNode.X, victimNode.Y);
                    targetNode.GoTo(victimNode.X, victimNode.Y);
                    victimNode.GoTo(targetNode.X, targetNode.Y);
                }
                ValidateAndFixTreeUI();
                // 5. Regenerate coordinates
                GenerateIndex(Root);


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
                var maxX = GetMaxX();
                var maxY = GetMaxY();
                var updatedSize = CoordCalc.GetNodeCoordinate(maxX + 1, maxY + 1);
                TreeCanvas.Width = updatedSize.X;
                TreeCanvas.Height = updatedSize.Y;

                Console.WriteLine("DeleteNodeInCanvas: Done");
            });
            Console.WriteLine("After DeleteNodeInCanvas: ");
            Console.WriteLine("- NodeList: " + NodeList.Count);
            foreach (var node in NodeList)
            {
                node.PrintNodeInfo();
            }
        }

        public void RedrawTree()
        {
            TreeCanvas.Children.Clear();
            GenerateIndex(Root);
            Queue<NodeUserControl> nodeQueue = new Queue<NodeUserControl>();
            nodeQueue.Append(Root);
            while (nodeQueue.Count > 0)
            {
                var current = nodeQueue.Dequeue();
                if (current == null)
                {
                    continue;
                }
                // Add itself
                TreeCanvas.Children.Add(current);
                _ = current.FadeIn();
                // Add line to Parent
                if (current.ParentNode != null)
                {
                    var line = new ConnectorLine();
                    line.Connect(current.ParentNode, current);
                    TreeCanvas.Children.Add(line);
                    line.AnimateAppear();
                }
                // Add Its Child
                if (current.LeftNode != null)
                {
                    // Add Child to Queue to be next current
                    nodeQueue.Append(current.LeftNode);
                }
                if (current.RightNode != null)
                {
                    // Add Child to Queue to be next current
                    nodeQueue.Append(current.RightNode);
                }
            }
        }


        public void ValidateAndFixTreeUI()
        {
            Console.WriteLine("Validate and Fix Tree:");
            Console.WriteLine($"Root: {Root?.Value}");
            Console.WriteLine($"NodeList: {NodeList.Count}");
            foreach (var node in NodeList)
                node.PrintNodeInfo();
            Console.WriteLine($"LineList: {LineList.Count}");
            var CoordCalc = MainWindow.CoordCalculator;
            List<NodeUserControl> delNodeList = new List<NodeUserControl>();
            // Find every Node that not exist in NodeList but still Exist on Canvas
            foreach (var child in TreeCanvas.Children)
            {
                if (child is NodeUserControl childNode && !NodeList.Contains(childNode))
                {
                    delNodeList.Add(childNode);
                }
            }

            // Find every Lione that not exist in NodeList but still Exist on Canvas
            List<ConnectorLine> delLineList = new List<ConnectorLine>();
            foreach (var node in delNodeList)
            {
                foreach (var child in TreeCanvas.Children)
                {
                    // Find invalid Line
                    if (child is ConnectorLine line && (line.StartElement == node || line.EndElement == node))
                    {
                        delLineList.Add(line);
                    }
                }
                // Remove node validated
                TreeCanvas.Children.Remove(node);
                // Remove line invalid
                foreach (var line in delLineList)
                {
                    TreeCanvas.Children.Remove(line);
                }
                // Clear List (avoid remove again)
                delLineList.Clear();
            }
            GenerateIndex(Root);
            if (Root != null)
            {
                var coord = CoordCalc.GetNodeCoordinate(Root.X, Root.Y);
                Root.GoTo(coord.X, coord.Y);

            }
            foreach (var node in NodeList)
            {
                // if Node in List but not Show => Show Node
                if (!TreeCanvas.Children.Contains(node))
                {
                    TreeCanvas.Children.Add(node);
                }
                // if showed => GoTo Destination
                var coord = CoordCalc.GetNodeCoordinate(node.X, node.Y);
                node.GoTo(coord.X, coord.Y);
                Console.WriteLine($"Node ({node.Value}) GoTo (X: {coord.X}, Y:{coord.Y})");

                Console.WriteLine($"Line: Node({node.Value})");
                // Validate Line
                Console.WriteLine($"Line: Parent(Val:{node.ParentNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node.ParentNode && line.EndElement == node).Count > 0})");
                if (node.ParentNode != null && LineList.FindAll(line => line.StartElement == node.ParentNode && line.EndElement == node).Count <= 0)
                {
                    var line = new ConnectorLine();
                    line.Connect(node.ParentNode, node);
                    LineList.Add(line);
                    line.AnimateAppear();
                    TreeCanvas.Children.Add(line);
                }

                Console.WriteLine($"Line: Left(Val:{node.LeftNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node && line.EndElement == node.LeftNode).Count > 0})");
                if (node.LeftNode != null && LineList.FindAll(line => line.StartElement == node && line.EndElement == node.LeftNode).Count <= 0)
                {
                    var line = new ConnectorLine();
                    line.Connect(node, node.LeftNode);
                    LineList.Add(line);
                    line.AnimateAppear();
                    TreeCanvas.Children.Add(line);
                }

                Console.WriteLine($"Line: Right(Val:{node.RightNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node && line.EndElement == node.RightNode).Count > 0})");
                if (node.RightNode != null && LineList.FindAll(line => line.StartElement == node && line.EndElement == node.RightNode).Count <= 0)
                {
                    var line = new ConnectorLine();
                    line.Connect(node, node.RightNode);
                    LineList.Add(line);
                    line.AnimateAppear();
                    TreeCanvas.Children.Add(line);
                }
            }
            var maxX = GetMaxX();
            var maxY = GetMaxY();
            var newSize = CoordCalc.GetNodeCoordinate(maxX + 1, maxY + 1);
            TreeCanvas.Width = newSize.X;
            TreeCanvas.Height = newSize.Y;
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

        public void SetStateFromRoot(NodeUserControl? current, NodeVisualState state)
        {
            while (current != null)
            {
                current.NodeState = state;
                current = current.ParentNode;
            }
        }

        public void SetStateToChild(NodeUserControl? current, NodeVisualState state)
        {
            if (current == null)
                return;

            current.NodeState = state;
            Thread.Sleep(10);
            SetStateToChild(current.LeftNode, state);
            SetStateToChild(current.RightNode, state);
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

        private NodeUserControl GetLeftMost(NodeUserControl node)
        {
            while (node.LeftNode != null)
                node = node.LeftNode;
            return node;
        }

        public void GoTo(double targetX, double targetY, double durationInSeconds = 0.5)
        {
            var currentX = Canvas.GetLeft(this);
            var currentY = Canvas.GetTop(this);

            if (double.IsNaN(currentX)) currentX = 0;
            if (double.IsNaN(currentY)) currentY = 0;

            var animX = new DoubleAnimation
            {
                From = currentX,
                To = targetX,
                Duration = TimeSpan.FromSeconds(durationInSeconds),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            var animY = new DoubleAnimation
            {
                From = currentY,
                To = targetY,
                Duration = TimeSpan.FromSeconds(durationInSeconds),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animX);
            storyboard.Children.Add(animY);

            Storyboard.SetTarget(animX, this);
            Storyboard.SetTarget(animY, this);

            Storyboard.SetTargetProperty(animX, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(animY, new PropertyPath("(Canvas.Top)"));

            storyboard.Begin();
        }

        public void SwapRelative(NodeUserControl node1, NodeUserControl node2)
        {
            if (node1 == null || node2 == null || node1 == node2)
                return;

            // Lưu lại các quan hệ ban đầu
            var parent1 = node1.ParentNode;
            var left1 = node1.LeftNode;
            var right1 = node1.RightNode;

            var parent2 = node2.ParentNode;
            var left2 = node2.LeftNode;
            var right2 = node2.RightNode;

            // Kiểm tra mối quan hệ cha – con trực tiếp
            bool node1IsChildOfNode2 = (left2 == node1 || right2 == node1);
            bool node2IsChildOfNode1 = (left1 == node2 || right1 == node2);

            // B1: Cập nhật quan hệ của cha với node1 và node2
            if (parent1 != null)
            {
                if (parent1.LeftNode == node1)
                    parent1.LeftNode = node2;
                else if (parent1.RightNode == node1)
                    parent1.RightNode = node2;
            }

            if (parent2 != null)
            {
                if (parent2.LeftNode == node2)
                    parent2.LeftNode = node1;
                else if (parent2.RightNode == node2)
                    parent2.RightNode = node1;
            }

            // B2: Cập nhật con của node1 (nếu node2 không phải là con)
            if (!node1IsChildOfNode2)
            {
                if (left1 != null && left1 != node2) left1.ParentNode = node2;
                if (right1 != null && right1 != node2) right1.ParentNode = node2;
            }

            // Cập nhật con của node2 (nếu node1 không phải là con)
            if (!node2IsChildOfNode1)
            {
                if (left2 != null && left2 != node1) left2.ParentNode = node1;
                if (right2 != null && right2 != node1) right2.ParentNode = node1;
            }

            // B3: Hoán đổi chính node1 và node2
            node1.ParentNode = node2IsChildOfNode1 ? node2 : parent2;
            node1.LeftNode = (left2 == node1) ? node2 : left2;
            node1.RightNode = (right2 == node1) ? node2 : right2;

            node2.ParentNode = node1IsChildOfNode2 ? node1 : parent1;
            node2.LeftNode = (left1 == node2) ? node1 : left1;
            node2.RightNode = (right1 == node2) ? node1 : right1;

            // B4: Cập nhật gốc nếu cần
            if (Root == node1)
                Root = node2;
            else if (Root == node2)
                Root = node1;
            Console.WriteLine($"Swapping Node:");
            Console.WriteLine($"Node1: Node({node1.Value})");
            Console.WriteLine($" - Parent(Val:{node1.ParentNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node1.ParentNode && line.EndElement == node1).Count > 0})");
            Console.WriteLine($" - Left(Val:{node1.LeftNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node1 && line.EndElement == node1.LeftNode).Count > 0})");
            Console.WriteLine($" - Right(Val:{node1.RightNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node1 && line.EndElement == node1.RightNode).Count > 0})");
            Console.WriteLine($"Node2: Node({node2.Value})");
            Console.WriteLine($" - Parent(Val:{node2.ParentNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node2.ParentNode && line.EndElement == node2).Count > 0})");
            Console.WriteLine($" - Left(Val:{node2.LeftNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node2 && line.EndElement == node2.LeftNode).Count > 0})");
            Console.WriteLine($" - Right(Val:{node2.RightNode?.Value}, HasLine: {LineList.FindAll(line => line.StartElement == node2 && line.EndElement == node2.RightNode).Count > 0})");
        }


        public void SwapAndValidateNode(NodeUserControl node1, NodeUserControl node2)
        {
            SwapRelative(node2, node2);
            ValidateAndFixTreeUI();
        }
        public int GetMaxX()
        {
            return GetMaxX(Root);
        }

        public void ClearRelative(NodeUserControl node)
        {
            if (node == null)
                return;

            // Ngắt quan hệ với cha
            if (node.ParentNode != null)
            {
                if (node.ParentNode.LeftNode == node)
                    node.ParentNode.LeftNode = null;
                else if (node.ParentNode.RightNode == node)
                    node.ParentNode.RightNode = null;
            }
            node.ParentNode = null;

            // Ngắt quan hệ với con trái
            if (node.LeftNode != null)
            {
                node.LeftNode.ParentNode = null;
            }
            node.LeftNode = null;

            // Ngắt quan hệ với con phải
            if (node.RightNode != null)
            {
                node.RightNode.ParentNode = null;
            }
            node.RightNode = null;
        }

        private int GetMaxX(NodeUserControl? node)
        {
            if (node == null) return 0;

            int x = node.X;
            int leftMax = GetMaxX(node.LeftNode);
            int rightMax = GetMaxX(node.RightNode);

            return Math.Max(x, Math.Max(leftMax, rightMax));
        }

        public int GetMaxY()
        {
            return GetMaxY(Root);
        }

        private int GetMaxY(NodeUserControl? node)
        {
            if (node == null) return 0;

            int y = node.Y;
            int leftMax = GetMaxY(node.LeftNode);
            int rightMax = GetMaxY(node.RightNode);

            return Math.Max(y, Math.Max(leftMax, rightMax));
        }

        public int GetMinX()
        {
            return GetMinX(Root);
        }

        private int GetMinX(NodeUserControl? node)
        {
            if (node == null) return int.MaxValue;

            int x = node.X;
            int leftMin = GetMinX(node.LeftNode);
            int rightMin = GetMinX(node.RightNode);

            return Math.Min(x, Math.Min(leftMin, rightMin));
        }

        public int GetMinY()
        {
            return GetMinY(Root);
        }

        private int GetMinY(NodeUserControl? node)
        {
            if (node == null) return int.MaxValue;

            int y = node.Y;
            int leftMin = GetMinY(node.LeftNode);
            int rightMin = GetMinY(node.RightNode);

            return Math.Min(y, Math.Min(leftMin, rightMin));
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

        public bool HasValue(string value)
        {
            if (value == null) return false;
            value = value.Trim().ToLower();
            foreach (var node in NodeList)
                if (node.Value == value) return true;
            return false;
        }
    }
}
