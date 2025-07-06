using MultipleChoice.Utils;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using TreeVisualizer.Models;
using TreeVisualizer.Services;
using TreeVisualizer.Views;

namespace TreeVisualizer.Views
{
    public partial class LoginWindow : Window
    {
        private EmailService _emailService;
        private UserService _userService;
        private AuthenticationService _authenticationService;
        private string ConfirmCode;
        // countdown Re-send email
        private DispatcherTimer countdownTimer;
        private int countdownSeconds = 60;
        public LoginWindow()
        {
            InitializeComponent();
            InpLoginEmail.Text = "mathettatc3@gmail.com";
            InpLoginPassword.Password = "1";
        }
        private void BtnAnimationRegister_Click(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                To = -1920,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            SlideTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void BtnAnimationLogin_Click(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            SlideTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void InpRegisterEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InpRegisterEmail.Text == string.Empty)
            {
                InpRegisterEmail.BorderBrush = Brushes.Black;
                InpRegisterEmail.Foreground = Brushes.Black;
                LblRegisterEmail.Foreground = Brushes.Black;
                return;
            }


            if (!SecurityUtil.ValidateEmailFormat(InpRegisterEmail.Text))
            {
                InpRegisterEmail.BorderBrush = Brushes.Red;
                InpRegisterEmail.Foreground = Brushes.Red;
                LblRegisterEmail.Foreground = Brushes.Red;
                BtnSignUp.IsEnabled = false;
                BtnRegisterGetEmailCode.IsEnabled = false;
                return;
            }
            else
            {
                InpRegisterEmail.BorderBrush = Brushes.Green;
                InpRegisterEmail.Foreground = Brushes.Green;
                LblRegisterEmail.Foreground = Brushes.Green;
                BtnSignUp.IsEnabled = true;
                BtnRegisterGetEmailCode.IsEnabled = true;
                return;
            }
        }


        private void BtnRegisterGetEmailCode_Click(object sender, RoutedEventArgs e)
        {
            // if no email => required input
            if (InpRegisterEmail.Text == string.Empty)
            {
                MessageBox.Show("Error: Please input your email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // if invalid email => required re-input
            if (!BtnRegisterGetEmailCode.IsEnabled)
            {
                MessageBox.Show("Error: Please input valid email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_emailService == null)
            {
                _emailService = new EmailService();
            }

            ConfirmCode = SecurityUtil.GenerateCode();

            string subject = "Register Confirmation Code";

            string body = $@"
                <h2 style='color: #2E86C1;'>Welcome to MC Application!</h2>
                <p>Your verification code is: <strong style='font-size: 20px;'>{ConfirmCode}</strong></p>
                <p>Thank you for using our service.</p>
            ";

            if (!_emailService.SendEmail(InpRegisterEmail.Text.Trim(), subject, body))
                MessageBox.Show("Error: Cannot send code to your email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            MessageBox.Show("Confirmation code has been sent to your email");

            // Khởi tạo timer
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            BtnRegisterGetEmailCode.Content = $"({countdownSeconds}s)";
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownSeconds--;

            if (countdownSeconds <= 0)
            {
                countdownTimer.Stop();
                BtnRegisterGetEmailCode.IsEnabled = true;
                BtnRegisterGetEmailCode.Content = "Resend";
                countdownSeconds = 60; // Reset nếu cần dùng lại
            }
            else
            {
                BtnRegisterGetEmailCode.Content = $"({countdownSeconds}s)";
            }
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            // Check Username
            if (InpRegisterUsername.Text == string.Empty)
            {
                MessageBox.Show("Error: Please enter username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LblRegisterUsername.Foreground = Brushes.Red;
                InpRegisterUsername.Foreground = Brushes.Red;
                InpRegisterUsername.BorderBrush = Brushes.Red;
                return;
            }

            // Check Email
            if (InpRegisterEmail.Text == string.Empty || !SecurityUtil.ValidateEmailFormat(InpRegisterEmail.Text))
            {
                MessageBox.Show("Error: Please input valid email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check Confirm Code
            if (!ConfirmCode.Equals(InpRegisterEmailCode.Text))
            {
                MessageBox.Show("Error: Wrong email confirmation code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                InpRegisterEmailCode.Foreground = Brushes.Red;
                InpRegisterEmailCode.BorderBrush = Brushes.Red;
                return;
            }

            // Reset Format
            InpRegisterEmailCode.Foreground = Brushes.Black;
            InpRegisterEmailCode.BorderBrush = Brushes.Black;

            // Check Password and ConfirmPassword
            if (InpRegisterPassword.Password == string.Empty)
            {
                MessageBox.Show("Error: Please enter password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LblRegisterPassword.Foreground = Brushes.Red;
                InpRegisterPassword.Foreground = Brushes.Red;
                InpRegisterPassword.BorderBrush = Brushes.Red;
                return;
            }

            // Reset Format
            LblRegisterPassword.Foreground = Brushes.Black;
            InpRegisterPassword.Foreground = Brushes.Black;
            InpRegisterPassword.BorderBrush = Brushes.Black;

            if (InpRegisterConfirmPassword.Password == string.Empty)
            {
                MessageBox.Show("Error: Please enter password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LblRegisterConfirmPassword.Foreground = Brushes.Red;
                InpRegisterConfirmPassword.Foreground = Brushes.Red;
                InpRegisterConfirmPassword.BorderBrush = Brushes.Red;
                return;
            }

            //Reset Format
            LblRegisterConfirmPassword.Foreground = Brushes.Red;
            InpRegisterConfirmPassword.Foreground = Brushes.Red;
            InpRegisterConfirmPassword.BorderBrush = Brushes.Red;

            if (InpRegisterConfirmPassword.Password != InpRegisterPassword.Password)
            {
                MessageBox.Show("Error: Password and Confirm Password must be the same", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LblRegisterPassword.Foreground = Brushes.Red;
                InpRegisterPassword.Foreground = Brushes.Red;
                InpRegisterPassword.BorderBrush = Brushes.Red;
                LblRegisterConfirmPassword.Foreground = Brushes.Red;
                InpRegisterConfirmPassword.Foreground = Brushes.Red;
                InpRegisterConfirmPassword.BorderBrush = Brushes.Red;
                return;
            }

            // Reset Format
            LblRegisterPassword.Foreground = Brushes.Black;
            LblRegisterConfirmPassword.Foreground = Brushes.Black;
            InpRegisterPassword.Foreground = Brushes.Black;
            InpRegisterConfirmPassword.Foreground = Brushes.Black;
            InpRegisterPassword.BorderBrush = Brushes.Black;
            InpRegisterConfirmPassword.BorderBrush = Brushes.Black;

            if (_userService == null)
            {
                _userService = new UserService();
            }
            if (_userService.GetByUsername(InpRegisterUsername.Text.Trim()) != null)
            {
                MessageBox.Show("Error: Username has been used", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_userService.GetByEmail(InpRegisterEmail.Text.Trim()) != null)
            {
                MessageBox.Show("Error: Email has been used", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var user = new User
            {
                Username = InpRegisterUsername.Text.Trim(),
                Email = InpRegisterEmail.Text.Trim(),
                Password = InpRegisterPassword.Password.Trim(),
            };
            if (_userService.Create(user))
            {
                MessageBox.Show("Message: Successfully created new User", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnAnimationLogin_Click(BtnAnimationLogin, new RoutedEventArgs());
            }
            else
            {
                MessageBox.Show("Error: Failed to created new User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void InpLoginEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InpLoginEmail.Text == string.Empty)
            {
                InpLoginEmail.BorderBrush = Brushes.Black;
                InpLoginEmail.Foreground = Brushes.Black;
                LblLoginEmail.Foreground = Brushes.Black;
                return;
            }


            if (!SecurityUtil.ValidateEmailFormat(InpLoginEmail.Text))
            {
                InpLoginEmail.BorderBrush = Brushes.Red;
                InpLoginEmail.Foreground = Brushes.Red;
                LblLoginEmail.Foreground = Brushes.Red;
                BtnSignUp.IsEnabled = false;
                return;
            }
            else
            {
                InpLoginEmail.BorderBrush = Brushes.Green;
                InpLoginEmail.Foreground = Brushes.Green;
                LblLoginEmail.Foreground = Brushes.Green;
                BtnSignUp.IsEnabled = true;
                return;
            }
        }
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // Check Email
            if (InpLoginEmail.Text == string.Empty || !SecurityUtil.ValidateEmailFormat(InpLoginEmail.Text))
            {
                MessageBox.Show("Error: Please input valid email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                InpLoginEmail.BorderBrush = Brushes.Red;
                InpLoginEmail.Foreground = Brushes.Red;
                LblLoginEmail.Foreground = Brushes.Red;
                return;
            }

            // Reset Format
            InpLoginEmail.BorderBrush = Brushes.Black;
            InpLoginEmail.Foreground = Brushes.Black;
            LblLoginEmail.Foreground = Brushes.Black;

            // Check Password
            if (InpLoginPassword.Password == string.Empty)
            {
                MessageBox.Show("Error: Please input password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                InpLoginPassword.BorderBrush = Brushes.Red;
                InpLoginPassword.Foreground = Brushes.Red;
                LblLoginPassword.Foreground = Brushes.Red;
                return;
            }
            // Reset Format
            InpLoginPassword.BorderBrush = Brushes.Black;
            InpLoginPassword.Foreground = Brushes.Black;
            LblLoginPassword.Foreground = Brushes.Black;

            if (_authenticationService == null)
            {
                _authenticationService = new AuthenticationService();
            }

            int uId = _authenticationService.Login(InpLoginEmail.Text, InpLoginPassword.Password);
            if (uId <= 0)
            {
                MessageBox.Show("Error: Wrong authentication credentials", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                InpLoginEmail.BorderBrush = Brushes.Red;
                InpLoginEmail.Foreground = Brushes.Red;
                LblLoginEmail.Foreground = Brushes.Red;
                InpLoginPassword.BorderBrush = Brushes.Red;
                InpLoginPassword.Foreground = Brushes.Red;
                LblLoginPassword.Foreground = Brushes.Red;
                return;
            }
            //Reset Formatss
            InpLoginEmail.BorderBrush = Brushes.Black;
            InpLoginEmail.Foreground = Brushes.Black;
            LblLoginEmail.Foreground = Brushes.Black;
            InpLoginPassword.BorderBrush = Brushes.Black;
            InpLoginPassword.Foreground = Brushes.Black;
            LblLoginPassword.Foreground = Brushes.Black;

            var menuWindow = new MenuWindow(uId);
            menuWindow.Show();
            this.Close();
        }
    }
}
