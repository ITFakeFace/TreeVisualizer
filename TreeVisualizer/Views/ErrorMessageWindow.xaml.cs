using System.Windows;

namespace TreeVisualizer.Views
{
    public partial class ErrorMessageWindow : Window
    {
        public ErrorMessageWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
