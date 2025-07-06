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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeVisualizer.Components.QuizzComponent
{
    /// <summary>
    /// Interaction logic for QuizzAnalyzerUserControl.xaml
    /// </summary>
    public partial class QuizzAnalyzerUserControl : UserControl
    {
        /*private readonly Brush[] _levelColors = new Brush[]
        {
        Brushes.Red,      // Low
        Brushes.Orange,   // Average
        Brushes.Green,    // Good
        Brushes.Blue      // Excellent
        };*/
        private readonly Brush[] _levelColors = new Brush[]
        {
            (Brush)new BrushConverter().ConvertFromString("#DA3F45"),
            (Brush)new BrushConverter().ConvertFromString("#F6B750"),
            (Brush)new BrushConverter().ConvertFromString("#0A9DBB"),
            (Brush)new BrushConverter().ConvertFromString("#68C15A"),
        };
        public QuizzAnalyzerUserControl()
        {
            InitializeComponent();
        }

        public void SetData(int total, int low, int average, int good, int excellent)
        {
            HalfPieCanvas.Children.Clear();

            TxtQuizzCount.Text = total.ToString();

            double outerRadius = HalfPieCanvas.Width / 2;
            double innerRadius = outerRadius * 0.85;

            double centerX = HalfPieCanvas.Width / 2;
            double centerY = HalfPieCanvas.Height;

            if (total == 0)
            {
                // Vẽ 1 bán cầu màu xám
                var graySlice = CreateHalfPieSlice(centerX, centerY, outerRadius, 0, 180, Brushes.LightGray);
                HalfPieCanvas.Children.Add(graySlice);
            }
            else
            {
                double[] values = new double[] { low, average, good, excellent };
                double sum = values.Sum();
                double angle = 0;

                for (int i = 0; i < values.Length; i++)
                {
                    double sweepAngle = (values[i] / sum) * 180;
                    var path = CreateHalfPieSlice(centerX, centerY, outerRadius, angle, sweepAngle, _levelColors[i]);
                    HalfPieCanvas.Children.Add(path);
                    angle += sweepAngle;
                }
            }

            // Dù có dữ liệu hay không, đều vẽ bán cầu trắng ở giữa
            WhiteHalfCircle.Data = CreateHalfPieGeometry(centerX, centerY, innerRadius, 0, 180);
            WhiteHalfCircle.Width = HalfPieCanvas.Width;
            WhiteHalfCircle.Height = HalfPieCanvas.Height;
        }

        private Path CreateHalfPieSlice(double cx, double cy, double radius, double startAngle, double sweepAngle, Brush fill)
        {
            double startRadians = (Math.PI / 180) * startAngle;
            double endRadians = (Math.PI / 180) * (startAngle + sweepAngle);

            Point startPoint = new Point(
                cx + radius * Math.Cos(startRadians),
                cy - radius * Math.Sin(startRadians));

            Point endPoint = new Point(
                cx + radius * Math.Cos(endRadians),
                cy - radius * Math.Sin(endRadians));

            bool isLargeArc = sweepAngle > 180;

            var segment = new PathFigure
            {
                StartPoint = new Point(cx, cy),
                Segments = new PathSegmentCollection
        {
            new LineSegment(startPoint, true),
            new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Counterclockwise
            },
            new LineSegment(new Point(cx, cy), true)
        },
                IsClosed = true
            };

            var geometry = new PathGeometry();
            geometry.Figures.Add(segment);

            return new Path
            {
                Fill = fill,
                Data = geometry
            };
        }

        private Geometry CreateHalfPieGeometry(double cx, double cy, double radius, double startAngle, double sweepAngle)
        {
            double startRadians = (Math.PI / 180) * startAngle;
            double endRadians = (Math.PI / 180) * (startAngle + sweepAngle);

            Point startPoint = new Point(
                cx + radius * Math.Cos(startRadians),
                cy - radius * Math.Sin(startRadians));

            Point endPoint = new Point(
                cx + radius * Math.Cos(endRadians),
                cy - radius * Math.Sin(endRadians));

            bool isLargeArc = sweepAngle > 180;

            var segment = new PathFigure
            {
                StartPoint = new Point(cx, cy),
                Segments = new PathSegmentCollection
        {
            new LineSegment(startPoint, true),
            new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Counterclockwise
            },
            new LineSegment(new Point(cx, cy), true)
        },
                IsClosed = true
            };

            var geometry = new PathGeometry();
            geometry.Figures.Add(segment);
            return geometry;
        }

    }

}
