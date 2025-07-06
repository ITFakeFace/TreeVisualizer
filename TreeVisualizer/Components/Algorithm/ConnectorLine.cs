using System;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Shapes;

namespace TreeVisualizer.Components.Algorithm
{
    //public class ConnectorLine : Shape
    //{
    //    public UIElement StartElement
    //    {
    //        get => (UIElement)GetValue(StartElementProperty);
    //        set => SetValue(StartElementProperty, value);
    //    }

    //    public static readonly DependencyProperty StartElementProperty =
    //        DependencyProperty.Register(nameof(StartElement), typeof(UIElement), typeof(ConnectorLine),
    //            new PropertyMetadata(null, OnStartElementChanged));

    //    public UIElement EndElement
    //    {
    //        get => (UIElement)GetValue(EndElementProperty);
    //        set => SetValue(EndElementProperty, value);
    //    }

    //    public static readonly DependencyProperty EndElementProperty =
    //        DependencyProperty.Register(nameof(EndElement), typeof(UIElement), typeof(ConnectorLine),
    //            new PropertyMetadata(null, OnEndElementChanged));

    //    private static void OnStartElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        var line = (ConnectorLine)d;
    //        if (e.OldValue is UIElement oldElem)
    //            oldElem.LayoutUpdated -= line.OnRelatedElementLayoutUpdated;
    //        if (e.NewValue is UIElement newElem)
    //            newElem.LayoutUpdated += line.OnRelatedElementLayoutUpdated;

    //        //line.InvalidateVisual();
    //    }

    //    public ConnectorLine()
    //    {
    //        TriggerMouseDownEvent += ConnectorLine_MouseDown;
    //    }

    //    private void ConnectorLine_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        var CoordCalc = MainWindow.CoordCalculator;
    //        Console.WriteLine($"Clicked Line");
    //        Console.WriteLine($"From: (X:{Canvas.GetLeft(StartElement)}, Y:{Canvas.GetTop(StartElement)})");
    //        Console.WriteLine($"To: (X:{Canvas.GetLeft(EndElement)}, Y:{Canvas.GetTop(EndElement)})");
    //    }

    //    private static void OnEndElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        var line = (ConnectorLine)d;
    //        if (e.OldValue is UIElement oldElem)
    //            oldElem.LayoutUpdated -= line.OnRelatedElementLayoutUpdated;
    //        if (e.NewValue is UIElement newElem)
    //            newElem.LayoutUpdated += line.OnRelatedElementLayoutUpdated;
    //        //line.InvalidateVisual();
    //    }

    //    private void OnRelatedElementLayoutUpdated(object sender, EventArgs e)
    //    {
    //        InvalidateVisual();
    //    }

    //    protected override Geometry DefiningGeometry
    //    {
    //        get
    //        {
    //            if (StartElement == null || EndElement == null)
    //                return Geometry.Empty;

    //            Point p1 = GetCenter(StartElement);
    //            Point p2 = GetCenter(EndElement);

    //            return new LineGeometry(p1, p2);
    //        }
    //    }

    //    private Point GetCenter(UIElement element)
    //    {
    //        if (element == null)
    //            return new Point(0, 0);

    //        // Lấy vị trí element trên Canvas
    //        double left = Canvas.GetLeft(element);
    //        double top = Canvas.GetTop(element);

    //        if (double.IsNaN(left)) left = 0;
    //        if (double.IsNaN(top)) top = 0;

    //        if (element is FrameworkElement fe)
    //        {
    //            return new Point(left + fe.Width / 2, top + fe.Height / 2);
    //        }

    //        // Nếu không phải FrameworkElement thì chỉ trả tọa độ left, top
    //        return new Point(left, top);
    //    }

    //    // ============================
    //    //      ANIMATION METHODS
    //    // ============================

    //    public void AnimateAppear()
    //    {
    //        var animation = new DoubleAnimation(0, 1, GetAnimationDuration())
    //        {
    //            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
    //        };
    //        BeginAnimation(OpacityProperty, animation);
    //    }

    //    public void AnimateDisappear(Action? onCompleted = null)
    //    {
    //        var animation = new DoubleAnimation(Opacity, 0, GetAnimationDuration())
    //        {
    //            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
    //            FillBehavior = FillBehavior.Stop
    //        };

    //        animation.Completed += (s, e) =>
    //        {
    //            Opacity = 0;
    //            onCompleted?.Invoke();
    //        };

    //        BeginAnimation(OpacityProperty, animation);
    //    }

    //    private Duration GetAnimationDuration()
    //    {
    //        if (TryFindResource("LineAnimationDuration") is double seconds)
    //            return new Duration(TimeSpan.FromSeconds(seconds));

    //        return new Duration(TimeSpan.FromSeconds(0.3)); // mặc định
    //    }

    //    protected override void OnInitialized(EventArgs e)
    //    {
    //        base.OnInitialized(e);
    //        Stroke = Brushes.Black;
    //        StrokeThickness = 3;
    //        Opacity = 0; // khởi đầu ẩn, chờ AnimateAppear()
    //        Canvas.SetZIndex(this, -1);

    //        this.LayoutUpdated += (s, e) => InvalidateVisual();
    //        if (StartElement != null)
    //            StartElement.LayoutUpdated += OnRelatedElementLayoutUpdated;
    //        if (EndElement != null)
    //            EndElement.LayoutUpdated += OnRelatedElementLayoutUpdated;
    //    }

    //    public ConnectorLine Connect(UIElement Start, UIElement End)
    //    {
    //        this.StartElement = Start;
    //        this.EndElement = End;
    //        return this;
    //    }

    //    public void UpdateLine()
    //    {
    //        InvalidateVisual(); // ép vẽ lại line
    //    }
    //}
}

