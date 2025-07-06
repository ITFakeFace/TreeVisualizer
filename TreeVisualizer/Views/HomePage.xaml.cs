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
using TreeVisualizer.Components.QuizzComponent;
using TreeVisualizer.Models;
using TreeVisualizer.Services;

namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly QuizzService _quizzService = new QuizzService();
        public List<Quizz> quizzList { get; set; }
        public HomePage()
        {
            InitializeComponent();
            LoadListQuizz();
        }
        public void LoadListQuizz()
        {
            ListQuizzCard.Children.Clear();
            quizzList = _quizzService.GetAll();
            foreach (Quizz quizz in quizzList)
            {
                var card = new QuestionCardUserControl
                {
                    Quizz = quizz,
                    Margin = new Thickness(10)
                };

                ListQuizzCard.Children.Add(card);
            }
        }

    }
}
