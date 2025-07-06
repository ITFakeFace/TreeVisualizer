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
using TreeVisualizer.Models;
using TreeVisualizer.Services;
using TreeVisualizer.Views;

namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for ExamPage.xaml
    /// </summary>
    public partial class ExamPage : Page
    {
        int quizzID;
        int timeLimit;
        bool IsResultShowable;
        private QuizzService _quizzService = new QuizzService();
        private UserService _userService = new UserService();

        public ExamPage(int quizzID)
        {
            InitializeComponent();
            this.quizzID = quizzID;
            Quizz quizz = _quizzService.GetById(quizzID);
            Title.Text = quizz.Title;
            CreatedByText.Text = _userService.GetById(quizz.CreatedBy).Username.ToString();
            TypeText.Text = quizz.Type.ToString();
            AttemptText.Text = string.IsNullOrEmpty(quizz.AttempNumber?.ToString()) ? "Unlimited" : quizz.AttempNumber.ToString();
            TimeLimitText.Text = quizz.TimeLimit.ToString();
            StartAtText.Text = string.IsNullOrEmpty(quizz.StartAt?.ToString()) ? "Anytime" : quizz.StartAt?.ToString();
            ResultShowableText.Text = quizz.IsResultShowable ? "allow" : "not allow";
            this.timeLimit = (int)(quizz.TimeLimit?.TotalMinutes ?? 0);

            this.IsResultShowable = quizz.IsResultShowable;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            QuizzTakingWindow quizzTakingWindows = new QuizzTakingWindow(this.quizzID, (MenuWindow)parent, this.timeLimit, this.IsResultShowable);
            quizzTakingWindows.Show();
            parent.Hide();
        }   
    }
}
