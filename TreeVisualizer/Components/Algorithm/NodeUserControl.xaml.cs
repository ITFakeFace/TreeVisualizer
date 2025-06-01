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
        public NodeUserControl? LeftNode { get; set; } = null;
        public NodeUserControl? RightNode { get; set; } = null;
        public NodeUserControl()
        {
            InitializeComponent();
            this.Loaded += NodeUserControl_Loaded;
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

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(NodeUserControl),
                new PropertyMetadata(0.0, OnPositionChanged));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(NodeUserControl),
                new PropertyMetadata(0.0, OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeUserControl control)
            {
                // Grid Calculating Function
                Canvas.SetLeft(control, control.X);
                Canvas.SetTop(control, control.Y);
            }
        }

        private void NodeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSize();
            UpdateNodeBackground();
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
            var size = MeasureString(TextBlockValue);
            double diameter = Math.Max(size.Width, size.Height) + 20; // +padding
            this.Width = diameter;
            this.Height = diameter;

            EllipseNode.Width = diameter;
            EllipseNode.Height = diameter;
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
            var duration = GetAnimationDuration();

            var scaleUp = new DoubleAnimation(0, 1, duration)
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);

            this.Visibility = Visibility.Visible;
        }

        public void FadeOut(Action? onCompleted = null)
        {
            var duration = GetAnimationDuration();

            var scaleDown = new DoubleAnimation(1, 0, duration)
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            scaleDown.Completed += (s, e) =>
            {
                this.Visibility = Visibility.Collapsed;
                onCompleted?.Invoke();
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);
        }

        public void GoTo(float x, float y)
        {
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

        public int CountChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetLeftNode()) + CountChildNode(node.GetRightNode()) + 1;
        }

        public int CountLeftChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetLeftNode());
        }

        public int CountRightChildNode(NodeUserControl? node)
        {
            if (node == null)
                return 0;

            return CountChildNode(node.GetRightNode());
        }
    }
}
