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
using TreeVisualizer.Views;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace TreeVisualizer.Components.Algorithm
{
    /// <summary>
    /// Interaction logic for TreeUserControl.xaml
    /// </summary>
    public partial class TreeUserControl : UserControl
    {
        public int AddNodeTraversalDelay { get; set; } = 300;

        public List<NodeUserControl> TraversalList = new List<NodeUserControl>();
        public HashSet<NodeUserControl> NodeList = new HashSet<NodeUserControl>();
        public HashSet<ConnectorLine> LineList = new HashSet<ConnectorLine>();
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

            var coordCalc = VisualTreePage.CoordCalculator;
            var size = coordCalc.GridSize;

            // Gán vị trí tạm thời ban đầu cho node (xuất hiện nhẹ nhàng từ gần gốc)
            Coordinate appearPos = coordCalc.GetNodeCoordinate(Root == null || NodeList.Count == 1 ? 0 : Root.X + 2, 0);

            // Add node vào canvas & bắt đầu hiệu ứng hiện ra
            TreeCanvas.Children.Add(node);
            Canvas.SetLeft(node, appearPos.X);
            Canvas.SetTop(node, appearPos.Y);
            await node.FadeIn(); // Node xuất hiện

            // Đợi nhỏ (delay ngắn) để cảm giác từng bước mượt
            await Task.Delay(AddNodeTraversalDelay);

            // Gọi lại Validate để cập nhật vị trí node và tính lại X/Y
            ValidateAndFixTreeUI();

            // Di chuyển tất cả node về đúng chỗ của chúng
            foreach (var child in NodeList)
            {
                Coordinate dest = coordCalc.GetNodeCoordinate(child.X, child.Y);
                child.GoTo(dest.X, dest.Y);
            }

            // Cập nhật kích thước canvas sau khi biết vị trí lớn nhất
            GenerateIndex(Root);
            var maxX = GetMaxX();
            var maxY = GetMaxY();
            var updatedSize = coordCalc.GetNodeCoordinate(maxX + 1, maxY + 1);
            TreeCanvas.Width = updatedSize.X;
            TreeCanvas.Height = updatedSize.Y;

            // Nếu có node cha, thêm line kết nối mới
            if (node.ParentNode != null)
            {
                var line = new ConnectorLine();
                line.Connect(node.ParentNode, node);
                LineList.Add(line);
                TreeCanvas.Children.Add(line);
                line.AnimateAppear();
            }

            // Cập nhật lại tất cả đường kết nối
            foreach (var line in LineList)
            {
                line.UpdateLine();
            }
        }

        public void DeleteNodeInCanvas(NodeUserControl? targetNode, NodeUserControl? victimNode)
        {
            if (targetNode == null) return;

            // Get canvas size info
            var size = VisualTreePage.CoordCalculator.GridSize;
            Console.WriteLine("DeleteNodeInCanvas: NodeList.Count = " + NodeList.Count);
            Console.WriteLine("DeleteNodeInCanvas: GridSize = " + size);
            var CoordCalc = VisualTreePage.CoordCalculator;

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


                var coordCalc = VisualTreePage.CoordCalculator;
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

            var coordCalc = VisualTreePage.CoordCalculator;

            // Xóa Node không còn tồn tại
            List<NodeUserControl> delNodeList = new();
            foreach (var child in TreeCanvas.Children.OfType<NodeUserControl>())
                if (!NodeList.Contains(child))
                    delNodeList.Add(child);

            // Xóa Line không hợp lệ (cha sai hoặc trùng line)
            List<ConnectorLine> delLineList = new();
            var uniqueLines = new HashSet<(NodeUserControl, NodeUserControl)>();

            foreach (var line in LineList.ToList())
            {
                if (line.StartElement is not NodeUserControl start || line.EndElement is not NodeUserControl end)
                {
                    delLineList.Add(line);
                    continue;
                }

                // Kiểm tra đúng cha-con
                bool isValidParent =
                    (end.ParentNode == start && (start.LeftNode == end || start.RightNode == end));

                if (!isValidParent)
                {
                    delLineList.Add(line);
                    continue;
                }

                // Kiểm tra trùng (A->B đã tồn tại)
                var key = (start, end);
                if (!uniqueLines.Add(key))
                {
                    delLineList.Add(line);
                }
            }

            // Remove all lines invalid or duplicate
            foreach (var line in delLineList)
            {
                LineList.Remove(line);
                TreeCanvas.Children.Remove(line);
            }

            // Remove node không còn dùng
            foreach (var node in delNodeList)
            {
                foreach (var line in LineList.Where(l => l.StartElement == node || l.EndElement == node).ToList())
                {
                    LineList.Remove(line);
                    TreeCanvas.Children.Remove(line);
                }

                TreeCanvas.Children.Remove(node);
            }

            // Rebuild toàn bộ index + vị trí
            GenerateIndex(Root);
            if (Root != null)
            {
                var rootCoord = coordCalc.GetNodeCoordinate(Root.X, Root.Y);
                Root.GoTo(rootCoord.X, rootCoord.Y);
            }

            foreach (var node in NodeList)
            {
                if (!TreeCanvas.Children.Contains(node))
                    TreeCanvas.Children.Add(node);

                var coord = coordCalc.GetNodeCoordinate(node.X, node.Y);
                node.GoTo(coord.X, coord.Y);

                // Tạo các line nếu chưa tồn tại
                void TryAddLine(NodeUserControl? start, NodeUserControl? end)
                {
                    if (start == null || end == null) return;

                    bool exists = LineList.Any(line =>
                        line.StartElement == start && line.EndElement == end);

                    if (!exists)
                    {
                        var line = new ConnectorLine();
                        line.Connect(start, end);
                        LineList.Add(line);
                        TreeCanvas.Children.Add(line);
                        line.AnimateAppear();
                    }
                }

                TryAddLine(node.ParentNode, node);
                TryAddLine(node, node.LeftNode);
                TryAddLine(node, node.RightNode);
            }

            // Cập nhật lại canvas size
            var maxX = GetMaxX();
            var maxY = GetMaxY();
            var newSize = coordCalc.GetNodeCoordinate(maxX + 1, maxY + 1);
            TreeCanvas.Width = newSize.X;
            TreeCanvas.Height = newSize.Y;

            // Update lại toàn bộ line position
            foreach (var line in LineList)
                line.UpdateLine();

            Console.WriteLine("End Validate and Fix Tree\n");
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
                    TraverseLRN(Root, ref result);
                    break;
                case "NLR":
                    TraverseNLR(Root, ref result);
                    break;
                case "NRL":
                    TraverseNRL(Root, ref result);
                    break;
                case "RLN":
                    TraverseRLN(Root, ref result);
                    break;
                case "RNL":
                    TraverseRNL(Root, ref result);
                    break;
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

        private List<string> TraverseLRN(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            TraverseLNR(node.GetLeftNode(), ref result);
            TraverseLNR(node.GetRightNode(), ref result);
            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            return result;
        }

        private List<string> TraverseNLR(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            TraverseLNR(node.GetLeftNode(), ref result);
            TraverseLNR(node.GetRightNode(), ref result);
            return result;
        }

        private List<string> TraverseNRL(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            TraverseLNR(node.GetRightNode(), ref result);
            TraverseLNR(node.GetLeftNode(), ref result);
            return result;
        }

        private List<string> TraverseRLN(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            TraverseLNR(node.GetRightNode(), ref result);
            TraverseLNR(node.GetLeftNode(), ref result);
            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            return result;
        }

        private List<string> TraverseRNL(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            TraverseLNR(node.GetRightNode(), ref result);
            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            TraverseLNR(node.GetLeftNode(), ref result);
            return result;
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
            Console.WriteLine($" - Parent(Val:{node1.ParentNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node1.ParentNode && line.EndElement == node1).ToList().Count > 0})");
            Console.WriteLine($" - Left(Val:{node1.LeftNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node1 && line.EndElement == node1.LeftNode).ToList().Count > 0})");
            Console.WriteLine($" - Right(Val:{node1.RightNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node1 && line.EndElement == node1.RightNode).ToList().Count > 0})");
            Console.WriteLine($"Node2: Node({node2.Value})");
            Console.WriteLine($" - Parent(Val:{node2.ParentNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node2.ParentNode && line.EndElement == node2).ToList().Count > 0})");
            Console.WriteLine($" - Left(Val:{node2.LeftNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node2 && line.EndElement == node2.LeftNode).ToList().Count > 0})");
            Console.WriteLine($" - Right(Val:{node2.RightNode?.Value}, HasLine: {LineList.Where(line => line.StartElement == node2 && line.EndElement == node2.RightNode).ToList().Count > 0})");
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
