using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeVisualizer.Components.Algorithm
{
    /// <summary>
    /// Interaction logic for NodeUserControl.xaml
    /// </summary> 
    public enum NodeVisualState
    {
        Default,
        Traversal,
        Target
    }
    public partial class NodeUserControl : UserControl
    {

        public NodeUserControl? ParentNode { get; set; } = null;
        public NodeUserControl? LeftNode { get; set; } = null;
        public NodeUserControl? RightNode { get; set; } = null;
        public NodeUserControl()
        {
            InitializeComponent();
            InitializeEvents();
        }

        public void InitializeEvents()
        {
            this.Loaded += NodeUserControl_Loaded;
            this.MouseDown += NodeUserControl_MouseDown;
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NodeUserControl),
                new PropertyMetadata("Value", OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeUserControl control && control.IsLoaded)
            {
                control.UpdateSize();
            }
        }

        public NodeVisualState NodeState
        {
            get => (NodeVisualState)GetValue(NodeStateProperty);
            set => SetValue(NodeStateProperty, value);
        }

        public static readonly DependencyProperty NodeStateProperty =
            DependencyProperty.Register(nameof(NodeState), typeof(NodeVisualState), typeof(NodeUserControl),
                new PropertyMetadata(NodeVisualState.Default, OnNodeStateChanged));

        private static void OnNodeStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeUserControl control)
                control.UpdateNodeBackground();
        }

        public double FontSizeValue
        {
            get { return (double)GetValue(FontSizeValueProperty); }
            set { SetValue(FontSizeValueProperty, value); }
        }

        public static readonly DependencyProperty FontSizeValueProperty =
            DependencyProperty.Register(nameof(FontSizeValue), typeof(double), typeof(NodeUserControl), new PropertyMetadata(32.0));

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        //public int X
        //{
        //    get => (int)GetValue(XProperty);
        //    set => SetValue(XProperty, value);
        //}

        //public static readonly DependencyProperty XProperty =
        //    DependencyProperty.Register(nameof(X), typeof(int), typeof(NodeUserControl),
        //        new PropertyMetadata(0.0, OnPositionChanged));

        //public int Y
        //{
        //    get => (int)GetValue(YProperty);
        //    set => SetValue(YProperty, value);
        //}

        //public static readonly DependencyProperty YProperty =
        //    DependencyProperty.Register(nameof(Y), typeof(int), typeof(NodeUserControl),
        //        new PropertyMetadata(0.0, OnPositionChanged));

        //private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var CoordCalc = MainWindow.CoordCalculator;
        //    if (d is NodeUserControl control)
        //    {
        //        // Grid Calculating Function
        //        var point = CoordCalc.GetNodeCoordinate(control.X, control.Y);
        //        Canvas.SetLeft(control, point.X);
        //        Canvas.SetTop(control, point.Y);
        //    }
        //}

        private void NodeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSize();
            UpdateNodeBackground();
        }

        private void NodeUserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var CoordCalc = MainWindow.CoordCalculator;
            Console.WriteLine($"Clicked Node Coordiate: (X:{X}, Y:{Y}), " + CoordCalc.GetNodeCoordinate(X, Y));
        }

        public void SetTraversal(NodeUserControl node, bool status)
        {
            if (node == null)
                return;
            if (status)
                node.NodeState = NodeVisualState.Traversal;
            else
                node.NodeState = NodeVisualState.Default;
        }

        private void UpdateNodeBackground()
        {
            Brush background = TryFindResource(NodeState switch
            {
                NodeVisualState.Traversal => "NodeBackgroundTraversal",
                NodeVisualState.Target => "NodeBackgroundTarget",
                _ => "NodeBackground"
            }) as Brush ?? Brushes.Gray;

            EllipseNode.Fill = background;
        }

        private void UpdateSize()
        {
            double maxDiameter = EllipseNode.Width; // hoặc this.Width

            double currentFontSize = 32; // bắt đầu từ font lớn
            double minFontSize = 10;

            TextBlockValue.FontSize = currentFontSize;

            var size = MeasureString(TextBlockValue);

            while ((size.Width > maxDiameter - 10 || size.Height > maxDiameter - 10) && currentFontSize > minFontSize)
            {
                currentFontSize -= 1;
                TextBlockValue.FontSize = currentFontSize;
                size = MeasureString(TextBlockValue);
            }

            FontSizeValue = currentFontSize;
        }

        private Size MeasureString(TextBlock candidate)
        {
            var formattedText = new FormattedText(
                candidate.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(candidate.FontFamily, candidate.FontStyle, candidate.FontWeight, candidate.FontStretch),
                candidate.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(candidate).PixelsPerDip
            );
            return new Size(formattedText.Width, formattedText.Height);
        }

        private Duration GetAnimationDuration()
        {
            if (TryFindResource("NodeAnimationDuration") is double seconds)
            {
                return new Duration(TimeSpan.FromSeconds(seconds));
            }

            return new Duration(TimeSpan.FromSeconds(0.3));
        }

        public void FadeIn()
        {
            this.Visibility = Visibility.Visible;

            var duration = GetAnimationDuration();

            var fadeInAnimation = new DoubleAnimation(0, 1, duration)
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        public void FadeOut(Action? onCompleted = null)
        {
            var duration = GetAnimationDuration();

            var fadeOutAnimation = new DoubleAnimation(1, 0, duration)
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOutAnimation.Completed += (s, e) =>
            {
                this.Visibility = Visibility.Collapsed;
                onCompleted?.Invoke();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        public void GoTo(double x, double y)
        {
            // if no change => skip animation => save time
            var CoordCalc = MainWindow.CoordCalculator;
            var coord = CoordCalc.GetNodeCoordinate(X, Y);
            if (X == x && Y == y)
            {
                return;
            }
            var duration = GetAnimationDuration();

            var leftAnim = new DoubleAnimation
            {
                To = x,
                Duration = duration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var topAnim = new DoubleAnimation
            {
                To = y,
                Duration = duration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Apply animation to Canvas.Left and Canvas.Top
            this.BeginAnimation(Canvas.LeftProperty, leftAnim);
            this.BeginAnimation(Canvas.TopProperty, topAnim);
        }

        public NodeUserControl? GetLeftNode() { return this.LeftNode; }
        public NodeUserControl? GetRightNode() { return this.RightNode; }

        public string? GetLeftNodeValue() { return this.LeftNode?.Value; }
        public string? GetRightNodeValue() { return this.RightNode?.Value; }

        // Description:
        // Recursively counts the total number of child nodes starting from the given node,
        // including the node itself.
        // 
        // Parameters:
        //   node - The starting node to count from.
        //
        // Returns:
        //   The total number of descendant nodes, including the given node.
        //
        // Example:
        //   For a tree like:
        //         A
        //        / \
        //       B   C
        //   CountChildNode(A) returns 3
        //   CountChildNode(B) returns 1
        public int CountChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetLeftNode()) + CountChildNode(node.GetRightNode()) + 1;
        }

        // Description:
        // Counts the total number of descendant nodes on the left subtree of the given node.
        // It does not include the root node itself, only its left descendants.
        //
        // Parameters:
        //   node - The root node whose left subtree will be counted.
        //
        // Returns:
        //   The total number of nodes in the left subtree.
        public int CountLeftChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetLeftNode());
        }

        // Description:
        // Counts the total number of descendant nodes on the right subtree of the given node.
        // It does not include the root node itself, only its right descendants.
        //
        // Parameters:
        //   node - The root node whose right subtree will be counted.
        //
        // Returns:
        //   The total number of nodes in the right subtree.
        public int CountRightChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetRightNode());
        }
    }
}
