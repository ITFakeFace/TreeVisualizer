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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeVisualizer.Components.Algorithm
{
    /// <summary>
    /// Interaction logic for ConnectorLine.xaml
    /// </summary>
    public partial class ConnectorLine : UserControl
    {
        public UIElement StartElement
        {
            get => (UIElement)GetValue(StartElementProperty);
            set => SetValue(StartElementProperty, value);
        }

        public static readonly DependencyProperty StartElementProperty =
            DependencyProperty.Register(nameof(StartElement), typeof(UIElement), typeof(ConnectorLine),
                new PropertyMetadata(null, OnElementChanged));

        public UIElement EndElement
        {
            get => (UIElement)GetValue(EndElementProperty);
            set => SetValue(EndElementProperty, value);
        }

        public static readonly DependencyProperty EndElementProperty =
            DependencyProperty.Register(nameof(EndElement), typeof(UIElement), typeof(ConnectorLine),
                new PropertyMetadata(null, OnElementChanged));

        private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ConnectorLine)d;

            if (e.OldValue is UIElement oldElem)
                oldElem.LayoutUpdated -= control.OnLayoutUpdated;
            if (e.NewValue is UIElement newElem)
                newElem.LayoutUpdated += control.OnLayoutUpdated;

            control.UpdateLine();
        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            UpdateLine();
        }

        public ConnectorLine()
        {
            InitializeComponent();
            Loaded += (_, _) => UpdateLine();
            Canvas.SetZIndex(this, -1);
        }


        public ConnectorLine(UIElement StartEle, UIElement EndEle)
        {
            StartElement = StartEle;
            EndElement = EndEle;
            InitializeComponent();
            Loaded += (_, _) => UpdateLine();
            Canvas.SetZIndex(this, -1);
        }

        public void UpdateLine()
        {
            if (StartElement == null || EndElement == null)
                return;

            var p1 = GetCenter(StartElement);
            var p2 = GetCenter(EndElement);

            Connector.X1 = p1.X;
            Connector.Y1 = p1.Y;
            Connector.X2 = p2.X;
            Connector.Y2 = p2.Y;
        }

        private Point GetCenter(UIElement element)
        {
            if (element is FrameworkElement fe && VisualTreeHelper.GetParent(this) is Visual parent)
            {
                var relativeTo = parent as UIElement ?? Window.GetWindow(this);
                var transform = element.TransformToVisual(relativeTo);
                var topLeft = transform.Transform(new Point(0, 0));
                return new Point(topLeft.X + fe.ActualWidth / 2, topLeft.Y + fe.ActualHeight / 2);
            }

            return new Point(0, 0);
        }

        public void AnimateAppear()
        {
            var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            Connector.BeginAnimation(OpacityProperty, animation);
        }

        public void AnimateDisappear(Action? onCompleted = null)
        {
            var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, e) =>
            {
                Connector.Opacity = 0;
                onCompleted?.Invoke();
            };

            Connector.BeginAnimation(OpacityProperty, animation);
        }

        public void Connect(UIElement start, UIElement end)
        {
            StartElement = start;
            EndElement = end;
            UpdateLine();
        }
    }
}
