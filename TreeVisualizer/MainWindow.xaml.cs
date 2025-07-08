using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeVisualizer.Components.Algorithm;
using TreeVisualizer.Components.Algorithm.AVLTree;
using TreeVisualizer.Components.Algorithm.BinarySearchTree;
using TreeVisualizer.Components.Algorithm.BinaryTree;
using TreeVisualizer.Components.ToolBar;
using TreeVisualizer.Utils.Coordinator;

namespace TreeVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _zoom = 0.1;
        private bool _isUpdatingZoom = false;
        private Point _cursorPrevPos;
        private bool _isRightMousePressed = false;
        public static Dictionary<ToolBarMode, ToolBarItemUserControl> ModeMap;
        public static ToolBarMode BeforeMode { get; set; }
        public static CoordinateCalculator CoordCalculator { get; set; }
        public TreeUserControl Tree { get; set; }
        public TreeService _treeService;
        private int treeID = -1;
        public string treeType = "BINARYTREE"; // 0: Binary Tree, 1: Binary Search Tree, 2: AVL Tree
        public MainWindow()
        {
            InitializeComponent();
            InitializeProperties();
            InitializeEvents();
            _treeService = new TreeService();
            var console = new ConsoleProgram();
            console.Run();
        }

        private void InitializeProperties()
        {
            //GridSize = 75;
            //coordinateCalculator = new CoordinateCalculator(new Coordinate(1500, 800), GridSize);
            //NodeGUI<int>.Calculator = coordinateCalculator;
            CoordCalculator = new CoordinateCalculator(new Coordinate { X = canvas.Width, Y = canvas.Height });
            Tree = new BinaryTreeUserControl();
            ModeMap = new Dictionary<ToolBarMode, ToolBarItemUserControl> {
                { ToolBarMode.Create, ModeCreate },
                { ToolBarMode.Update, ModeUpdate },
                { ToolBarMode.Delete, ModeDelete },
                { ToolBarMode.Move, ModeMove },
                { ToolBarMode.Export, ModeSave },
                { ToolBarMode.Search, ModeSearch },
                { ToolBarMode.Import, ModeImport },
                { ToolBarMode.Traverse, ModeTraverse},
                { ToolBarMode.ChangeTreeType, ModeChangeTree},
            };
            canvas.Children.Add(Tree);
        }
        private void InitializeEvents()
        {
            foreach (ToolBarItemUserControl item in ModeMap.Values)
            {
                item.OnModeChange += OnModeChange;
            }

            //zoom
            MouseWheel += OnMouseWheelEvent;
            //move
            PreviewMouseDown += OnMouseEvent;
            PreviewMouseUp += OnMouseEvent;
            MouseMove += OnMouseMoveEvent;
            canvas.Loaded += InkCanvas_Loaded;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            RadioBinaryTree.Checked += OnChangeTreeType;
            RadioBSTree.Checked += OnChangeTreeType;
            RadioAVLTree.Checked += OnChangeTreeType;
        }

        public static ToolBarMode GetCurrentMode()
        {
            foreach (ToolBarMode key in ModeMap.Keys)
            {
                if (ModeMap[key].isActive)
                {
                    return key;
                }
            }
            return ToolBarMode.None;
        }
        void OnKeyDown(object sender, KeyEventArgs e)
        {

        }
        void OnKeyUp(object sender, KeyEventArgs e)
        {
            /*
			if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl))
			{
				ToolBarItemUC.DisableAll();
				if (BeforeMode != ToolBarMode.None)
				{
					ModeMap[BeforeMode].Enable();
				}
				BeforeMode = ToolBarMode.None;
				ChangeMode();
				Console.WriteLine("Up");
			}
			*/
            if (FocusManager.GetFocusedElement(this) == null)
            {
                switch (e.Key)
                {
                    case Key.M:
                        ToolBarItemUserControl.DisableAll();
                        if (GetCurrentMode() != ToolBarMode.Move)
                        {
                            ModeMove.Enable();
                        }
                        ChangeMode();
                        break;
                    default:
                        break;
                }
            }
        }

        void OnMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            if (ModeMap[ToolBarMode.Move].isActive)
            {
                var diff = e.Delta > 0 ? 0.1 : -0.1;
                _zoom = Math.Clamp(_zoom + diff, 0.1, 10);

                var pos = e.GetPosition(canvas);
                UpdateZoom(pos);
            }
        }

        private void UpdateZoom(Point pos)
        {
            if (ModeMap[ToolBarMode.Move].isActive)
            {
                _isUpdatingZoom = true;

                var matrix = canvas.RenderTransform.Value;

                var targetWidth = canvas.ActualWidth * _zoom * 10d;
                var targetHeight = canvas.ActualHeight * _zoom * 10d;

                var topLeft = canvas.TranslatePoint(new Point(0, 0), this);
                var bottomRight = canvas.TranslatePoint(new Point(canvas.ActualWidth, canvas.ActualHeight), this);

                var renderWidth = bottomRight.X - topLeft.X;
                var renderHeight = bottomRight.Y - topLeft.Y;

                var scaleX = targetWidth / renderWidth;
                var scaleY = targetHeight / renderHeight;

                matrix.ScaleAtPrepend(scaleX, scaleY, pos.X, pos.Y);
                if (matrix.OffsetX > 0)
                    matrix.Translate(-matrix.OffsetX, 0);
                if (matrix.OffsetY > 0)
                    matrix.Translate(0, -matrix.OffsetY);
                if (matrix.OffsetX + targetWidth < NodeContainerParent.ActualWidth)
                    matrix.Translate(NodeContainerParent.ActualWidth - (matrix.OffsetX + targetWidth), 0);
                if (matrix.OffsetY + targetHeight < NodeContainerParent.ActualHeight)
                    matrix.Translate(0, NodeContainerParent.ActualHeight - (matrix.OffsetY + targetHeight));

                canvas.RenderTransform = new MatrixTransform(matrix);
            }
        }

        private void InkCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            var matrix = canvas.RenderTransform.Value;
            matrix.ScaleAtPrepend(1, 1, NodeContainerParent.ActualWidth / 2, NodeContainerParent.ActualHeight / 2);
            canvas.RenderTransform = new MatrixTransform(matrix);
        }

        void OnMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (_isRightMousePressed && ModeMap[ToolBarMode.Move].isActive)
            {
                var cursorPoint = e.GetPosition(this);
                var vector = cursorPoint - _cursorPrevPos;
                _cursorPrevPos = cursorPoint;
                //canvas.Strokes.Transform(new Matrix(1, 0, 0, 1, vector.X * (0.1 / _zoom), vector.Y * (0.1 / _zoom)), false);
                Canvas.SetLeft(canvas, Canvas.GetLeft(canvas) + vector.X);
                Canvas.SetTop(canvas, Canvas.GetTop(canvas) + vector.Y);
            }
        }

        private void OnMouseEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && !_isRightMousePressed && ModeMap[ToolBarMode.Move].isActive)
            {
                _isRightMousePressed = true;
                _cursorPrevPos = e.GetPosition(this);
            }
            else if (e.RightButton == MouseButtonState.Released && ModeMap[ToolBarMode.Move].isActive)
            {
                _isRightMousePressed = false;
            }

        }

        private void ChangeMode()
        {
            ToolBarMode? currentMode = ToolBarMode.None;
            foreach (ToolBarMode mode in ModeMap.Keys)
            {
                if (ModeMap[mode].isActive)
                {
                    currentMode = mode;
                    break;
                }
            }
            AddMenu.Visibility = Visibility.Hidden;
            ChangeTypeMenu.Visibility = Visibility.Hidden;
            ChangeNodeMenu.Visibility = Visibility.Hidden;
            ExportMode.Visibility = Visibility.Hidden;
            ImportMode.Visibility = Visibility.Hidden;
            DeleteMenu.Visibility = Visibility.Hidden;
            TraverseMenu.Visibility = Visibility.Hidden;
            FindMenu.Visibility = Visibility.Hidden;
            int index = -1;
            switch (currentMode)
            {
                case ToolBarMode.Create:
                    index = 0;
                    AddMenu.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.Update:
                    index = 1;
                    ChangeNodeMenu.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.Delete:
                    index = 2;
                    DeleteMenu.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.Search:
                    index = 3;
                    FindMenu.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.Traverse:
                    index = 4;
                    TraverseMenu.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.Move:
                    index = -1;
                    break;
                case ToolBarMode.Export:
                    ExportMode.Visibility = Visibility.Visible;
                    index = -2;
                    break;
                case ToolBarMode.Import:
                    index = 7;
                    ImportMode.Visibility = Visibility.Visible;
                    break;
                case ToolBarMode.ChangeTreeType:
                    ChangeTypeMenu.Visibility = Visibility.Visible;
                    index = 8;
                    break;
                case ToolBarMode.None:
                default:
                    index = -1;
                    break;
            }
            if (index >= 0)
            {
                ToolBarMenu.Visibility = Visibility.Visible;
                Canvas.SetLeft(ToolBarCursor, 275 + 95 * index);
            }
            else
            {
                ToolBarMenu.Visibility = Visibility.Hidden;
            }
        }

        public void OnModeChange(object sender, EventArgs e)
        {
            ChangeMode();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void AddField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AddField.Text.ToUpper().Equals(""))
            {
                AddField.Text = "Insert";
            }
            SolidColorBrush brush = new SolidColorBrush();
            AddField.BorderThickness = BtnAdd.BorderThickness = new Thickness(2);
            AddField.BorderBrush = BtnAdd.BorderBrush = Brushes.Black;
        }

        private void AddField_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (AddField.Text.ToUpper().Equals("INSERT"))
            {
                AddField.Text = "";
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            AddField.BorderThickness = BtnAdd.BorderThickness = new Thickness(4);
            AddField.BorderBrush = BtnAdd.BorderBrush = brush;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ToolBarMenuCanvas.Height <= 50)
            {
                MenuResizeButtonImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Static/Images/NavBar/Up_100px.png"));
                ToolBarMenuCanvas.Height = 150;
                NavMenyPanel.Visibility = Visibility.Visible;
            }
            else if (ToolBarMenuCanvas.Height > 50)
            {
                MenuResizeButtonImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Static/Images/NavBar/Down_100px.png"));
                ToolBarMenuCanvas.Height = 50;
                NavMenyPanel.Visibility = Visibility.Hidden;
            }
        }

        private void AddField_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (CreateNode())
                    {
                        AddField.Text = "";
                    }
                    else
                    {
                        new Thread(() =>
                        {
                            String oldValue = "";
                            this.Dispatcher.Invoke(() =>
                            {
                                oldValue = AddField.Text;
                                AddField.Text = "Error";
                                AddField.IsEnabled = false;
                            });
                            Thread.Sleep(2000);
                            this.Dispatcher.Invoke(() =>
                            {
                                if (AddField.Text.Trim().CompareTo("Error") == 0)
                                {
                                    AddField.IsEnabled = true;
                                    AddField.Text = oldValue;
                                }
                            });
                        }).Start();
                    }
                    break;
                case Key.Tab:
                    BtnAdd.Focus();
                    break;
                default:
                    break;
            }
        }

        private bool CreateNode()
        {
            try
            {
                var value = AddField.Text.Trim();
                int.Parse(value);
                Tree.AddNode(value);
                JustifyCenterTree();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainWindow: Exception when Create: " + ex.Message);
                return false;
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {

        }

        public void DeleteSubmit_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void DeleteSubmit_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Delete Btn Clicked");
            Tree.RemoveNode(DeleteValue.Text);
            JustifyCenterTree();
        }

        private void AmountGenField_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AmountGenField.Text.Trim().ToUpper().Equals("AMOUNT"))
            {
                AmountGenField.Text = "";
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);
        }

        private void AmountGenField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AmountGenField.Text.Trim().ToUpper().Equals(""))
            {
                AmountGenField.Text = "Amount";
            }
            BtnGenerate.BorderBrush = Brushes.Black;
            BtnGenerate.BorderThickness = new Thickness(2);
        }

        private void MinGenField_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MinGenField.Text.Trim().ToUpper().Equals("MIN"))
            {
                MinGenField.Text = "";
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);
        }

        private void MinGenField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MinGenField.Text.Trim().ToUpper().Equals(""))
            {
                MinGenField.Text = "Min";
            }
            BtnGenerate.BorderBrush = Brushes.Black;
            BtnGenerate.BorderThickness = new Thickness(2);
        }

        private void MaxGenField_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MaxGenField.Text.Trim().ToUpper().Equals("MAX"))
            {
                MaxGenField.Text = "";
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);
        }

        private void MaxGenField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MaxGenField.Text.Trim().ToUpper().Equals(""))
            {
                MaxGenField.Text = "Max";
            }
            BtnGenerate.BorderBrush = Brushes.Black;
            BtnGenerate.BorderThickness = new Thickness(2);
        }

        private void BtnGenerate_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);
        }

        private void BtnGenerate_LostFocus(object sender, RoutedEventArgs e)
        {
            BtnGenerate.BorderBrush = Brushes.Black;
            BtnGenerate.BorderThickness = new Thickness(2);
        }

        private void BtnAdd_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnAdd.BorderBrush = brush;
            BtnAdd.BorderThickness = new Thickness(4);
        }

        private void BtnAdd_LostFocus(object sender, RoutedEventArgs e)
        {
            BtnAdd.BorderBrush = Brushes.Black;
            BtnAdd.BorderThickness = new Thickness(2);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            CreateNode();
        }

        private void TempBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        #region before changeNodeVal
        private void BeforeChangeField_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    int inpBeforeChange = int.Parse(BeforeChangeField.Text.ToString());
                    AfterChangeField.Focus();
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Argument Null Exception In AddField");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Input Format Exception In AddField");
            }
            catch (OverflowException ex)
            {
                Console.WriteLine("Out of limit Exception In AddField");
            }
        }

        private void BeforeChangeField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (BeforeChangeField.Text is null || BeforeChangeField.Text == "")
            {
                BeforeChangeField.Text = "Before";
            }
        }
        private void BeforeChangeField_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);

        }

        private void BeforeChangeField_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnGenerate.BorderBrush = brush;
            BtnGenerate.BorderThickness = new Thickness(4);
            if (BeforeChangeField.Text.ToLower().Equals("before"))
            {
                BeforeChangeField.Text = null;
            }
        }

        private async void OnChangeTreeType(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBeforeChange_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AfterChangeField.Focus();
            }
        }
        #endregion

        private void ChangeNodeMenu_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBeforeChange_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            BtnBeforeChange.BorderBrush = brush;
            BtnBeforeChange.BorderThickness = new Thickness(4);

        }

        #region after change node

        private void AfterChangeField_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AfterChangeField.Text.ToUpper().Equals("AFTER"))
            {
                AfterChangeField.Text = "";
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString("#00d2ff");
            AddField.BorderThickness = BtnAdd.BorderThickness = new Thickness(4);
            AddField.BorderBrush = BtnAdd.BorderBrush = brush;
        }

        private void AfterChangeField_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion



        private void SaveAsTxtBtn_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ImportFileBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReadFileResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImportByText_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ImportByText_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBeforeChange_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAfter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLNR_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnLRN_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnNLR_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnNRL_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnRLN_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnRNL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {

        }

        private void JustifyCenterTree()
        {
            var treeSize = CoordCalculator.GetNodeCoordinate(Tree.GetMaxX() + 1, Tree.GetMaxY() + 1);
            double canvasCenterX = (canvas.ActualWidth - treeSize.X) / 2;
            double canvasCenterY = (canvas.ActualHeight - treeSize.Y) / 2; // hoặc tính toán tùy node root

            Tree.GoTo(canvasCenterX, canvasCenterY);
        }

        private void RadioBinaryTree_Checked(object sender, RoutedEventArgs e)
        {
            Tree = new BinaryTreeUserControl();
            this.treeType = "BINARYTREE";
            canvas.Children.Clear();
            canvas.Children.Add(Tree);
        }

        private void RadioBSTree_Checked(object sender, RoutedEventArgs e)
        {
            Tree = new BinarySearchTreeUserControl();
            this.treeType = "BINARYSEARCHTREE";
            canvas.Children.Clear();
            canvas.Children.Add(Tree);
        }

        private void RadioAVLTree_Checked(object sender, RoutedEventArgs e)
        {
            Tree = new AVLTreeUserControl();
            canvas.Children.Clear();
            canvas.Children.Add(Tree);
        }

        private void ModeSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Tree.Root == null)
            {
                new ErrorMessageWindow("Tree is empty. Cannot save.").ShowDialog();
                return;
            }
            if(treeID == -1)
            {
                new SaveTreeWindow(Tree.Root,this.treeType).ShowDialog();
                return;
            }
        }
    }
}