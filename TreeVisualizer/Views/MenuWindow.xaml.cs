using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TreeVisualizer;
using TreeVisualizer.Views;

namespace TreeVisualizer.Views
{
    public partial class MenuWindow : Window
    {
        private HomePage _homePage;
        private QuizzManagementPage _quizzPage;
        private AccountDetailPage _accountDetailPage;
        public static int UserId;
        public MenuWindow(int userId)
        {
            InitializeComponent();
            UserId = userId;
            Console.WriteLine($"UserId: {UserId}"); 
            _homePage = new HomePage();
            _quizzPage = new QuizzManagementPage();
            _accountDetailPage = new AccountDetailPage();
            MainFrame.Navigate(_homePage);
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            ExpandSidebar();
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            CollapseSidebar();
        }

        private void ExpandSidebar()
        {
            int maxWidth = 220;
            int horizontalGap = 10;
            SidebarColumn.Width = new GridLength(maxWidth);

            // Hiện text
            HomeText.Visibility = Visibility.Visible;
            SettingsText.Visibility = Visibility.Visible;
            DashboardText.Visibility = Visibility.Visible;
            LogoutText.Visibility = Visibility.Visible;
            TreeVisualizeText.Visibility = Visibility.Visible;
            // Update nút
            var realWidth = maxWidth - horizontalGap / 2;
            UpdateSidebarButton(BtnHome, HorizontalAlignment.Left, realWidth, new Thickness(10, 0, 10, 0));
            UpdateSidebarButton(BtnAccount, HorizontalAlignment.Left, realWidth, new Thickness(10, 0, 10, 0));
            UpdateSidebarButton(BtnQuizz, HorizontalAlignment.Left, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnLogout, HorizontalAlignment.Left, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnTreeVisualize, HorizontalAlignment.Left, realWidth, new Thickness(10, 0, 10, 0));
        }

        private void CollapseSidebar()
        {
            int maxWidth = 80;
            int horizontalGap = 10;
            SidebarColumn.Width = new GridLength(maxWidth);

            // Ẩn text
            HomeText.Visibility = Visibility.Collapsed;
            SettingsText.Visibility = Visibility.Collapsed;
            DashboardText.Visibility = Visibility.Collapsed;
            LogoutText.Visibility = Visibility.Collapsed;
            TreeVisualizeText.Visibility = Visibility.Collapsed;
            // Reset lại nút
            var realWidth = maxWidth - horizontalGap / 2;
            UpdateSidebarButton(BtnHome, HorizontalAlignment.Center, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnAccount, HorizontalAlignment.Center, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnQuizz, HorizontalAlignment.Center, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnLogout, HorizontalAlignment.Center, realWidth, new Thickness(10));
            UpdateSidebarButton(BtnTreeVisualize, HorizontalAlignment.Center, realWidth, new Thickness(10));
        }

        private void UpdateSidebarButton(Button button, HorizontalAlignment alignment, double width, Thickness padding)
        {
            button.HorizontalContentAlignment = alignment;
            button.Width = width;
            button.Padding = padding;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            TxtPageTitle.Text = "Home Page";
            _homePage.LoadListQuizz();
            MainFrame.Navigate(_homePage);
        }

        private void BtnQuizz_Click(object sender, RoutedEventArgs e)
        {
            TxtPageTitle.Text = "Owned Quizz Management";
            MainFrame.Navigate(_quizzPage);
        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {
            TxtPageTitle.Text = "Account Information";
            MainFrame.Navigate(_accountDetailPage);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnTreeVisualize_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
