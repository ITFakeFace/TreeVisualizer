using System;
using System.Windows;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Shapes;

namespace TreeVisualizer.Components.Algorithm
{
    public class ConnectorLine : Shape
    {
        public UIElement StartElement
        {
            get => (UIElement)GetValue(StartElementProperty);
            set => SetValue(StartElementProperty, value);
        }

        public static readonly DependencyProperty StartElementProperty =
            DependencyProperty.Register(nameof(StartElement), typeof(UIElement), typeof(ConnectorLine),
                new PropertyMetadata(null, OnStartElementChanged));

        public UIElement EndElement
        {
            get => (UIElement)GetValue(EndElementProperty);
            set => SetValue(EndElementProperty, value);
        }

        public static readonly DependencyProperty EndElementProperty =
            DependencyProperty.Register(nameof(EndElement), typeof(UIElement), typeof(ConnectorLine),
                new PropertyMetadata(null, OnEndElementChanged));

        private static void OnStartElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (ConnectorLine)d;
            if (e.OldValue is UIElement oldElem)
                oldElem.LayoutUpdated -= line.OnRelatedElementLayoutUpdated;
            if (e.NewValue is UIElement newElem)
                newElem.LayoutUpdated += line.OnRelatedElementLayoutUpdated;

            line.InvalidateVisual();
        }

        private static void OnEndElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (ConnectorLine)d;
            if (e.OldValue is UIElement oldElem)
                oldElem.LayoutUpdated -= line.OnRelatedElementLayoutUpdated;
            if (e.NewValue is UIElement newElem)
                newElem.LayoutUpdated += line.OnRelatedElementLayoutUpdated;

            line.InvalidateVisual();
        }

        private void OnRelatedElementLayoutUpdated(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (StartElement == null || EndElement == null)
                    return Geometry.Empty;

                Point p1 = GetCenter(StartElement);
                Point p2 = GetCenter(EndElement);

                return new LineGeometry(p1, p2);
            }
        }

        private Point GetCenter(UIElement element)
        {
            if (element == null || VisualTreeHelper.GetParent(element) is not Visual root)
                return new Point(0, 0);

            var transform = element.TransformToAncestor(root);
            var size = (element as FrameworkElement)?.RenderSize ?? new Size(0, 0);
            return transform.Transform(new Point(size.Width / 2, size.Height / 2));
        }

        // ============================
        //      ANIMATION METHODS
        // ============================

        public void AnimateAppear()
        {
            var animation = new DoubleAnimation(0, 1, GetAnimationDuration())
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            BeginAnimation(OpacityProperty, animation);
        }

        public void AnimateDisappear(Action? onCompleted = null)
        {
            var animation = new DoubleAnimation(Opacity, 0, GetAnimationDuration())
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, e) =>
            {
                Opacity = 0;
                onCompleted?.Invoke();
            };

            BeginAnimation(OpacityProperty, animation);
        }

        private Duration GetAnimationDuration()
        {
            if (TryFindResource("LineAnimationDuration") is double seconds)
                return new Duration(TimeSpan.FromSeconds(seconds));

            return new Duration(TimeSpan.FromSeconds(0.3)); // mặc định
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Stroke = Brushes.Black;
            StrokeThickness = 2;
            Opacity = 0; // khởi đầu ẩn, chờ AnimateAppear()
        }
    }
}

