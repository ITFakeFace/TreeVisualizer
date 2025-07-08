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
using TreeVisualizer.Views;

namespace TreeVisualizer.Components.ToolBar
{
    /// <summary>
    /// Interaction logic for ToolBarItemUserControl.xaml
    /// </summary>

    public enum ToolBarMode
    {
        Create, Update, Delete, Move, Export, Import, Select, Search, ChangeTreeType, Traverse, None
    }
    public partial class ToolBarItemUserControl : UserControl
    {
        public event EventHandler OnModeChange;
        public static readonly DependencyProperty ModeTypeProperty = DependencyProperty.Register("Mode", typeof(ToolBarMode), typeof(ToolBarItemUserControl), new PropertyMetadata(ToolBarMode.Create, ModeTypePropertyChanged));
        public ToolBarMode Mode
        {
            get { return (ToolBarMode)GetValue(ModeTypeProperty); }
            set { SetValue(ModeTypeProperty, value); }
        }

        public static void ModeTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToolBarItemUserControl)d;
            control.UpdateItemImage();
        }
        public bool isActive = false;

        public void UpdateItemImage()
        {
            String ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Add_100px.png";
            switch (Mode)
            {
                case ToolBarMode.Create:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Add_100px.png";
                    break;
                case ToolBarMode.Update:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Edit_100px.png";
                    break;
                case ToolBarMode.Delete:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Remove_100px.png";
                    break;
                case ToolBarMode.Move:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Expand_100px.png";
                    break;
                case ToolBarMode.Search:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Search_100px.png";
                    break;
                case ToolBarMode.Export:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Export_512px.png";
                    break;
                case ToolBarMode.Import:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Import_512px.png";
                    break;
                case ToolBarMode.ChangeTreeType:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/BinaryTree_1300px.png";
                    break;
                case ToolBarMode.Traverse:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Traversal_1300px.png";
                    break;
                case ToolBarMode.None:
                default:
                    ImagePath = "pack://application:,,,/Resources/Static/Images/ToolBar/Error_100px.png";
                    break;
            }
            ItemImage.Source = new BitmapImage(new Uri(ImagePath));
        }
        public ToolBarItemUserControl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        public void InitializeEvent()
        {
            MouseDown += OnClick;
        }

        public void OnClick(object sender, MouseEventArgs e)
        {
            if (!isActive)
            {
                Enable();
            }
            else
            {
                Disable();
            }
            if (this.OnModeChange != null)
                this.OnModeChange(this, new EventArgs());
        }

        public void Enable()
        {
            isActive = true;
            DisableAll();
            VisualTreePage.ModeMap[Mode].isActive = true;
            //RadialGradientBrush GradientBrush = new RadialGradientBrush();
            //GradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 1));
            //GradientBrush.GradientStops.Add(new GradientStop(Colors.Pink, 1));
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            //linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#12c2e9"), 0.2));
            //linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#c471ed"), 0.6));
            //linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#f64f59"), 1));
            linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#00d2ff"), 0.3));
            linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#928DAB"), 1));
            ItemBorder.Background = linearGradientBrush;
        }
        public void Disable()
        {
            isActive = false;
            VisualTreePage.ModeMap[Mode].isActive = false;
            SolidColorBrush SolidBrush = new SolidColorBrush();
            SolidBrush.Color = Colors.White;
            ItemBorder.Background = SolidBrush;
        }

        public static void DisableAll()
        {
            foreach (ToolBarMode Mode in VisualTreePage.ModeMap.Keys)
            {
                VisualTreePage.ModeMap[Mode].isActive = false;
                VisualTreePage.ModeMap[Mode].Disable();
            }
        }
    }
}
