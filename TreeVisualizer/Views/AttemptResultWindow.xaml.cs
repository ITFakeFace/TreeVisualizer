using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TreeVisualizer.Models;
using TreeVisualizer.Services;
using TreeVisualizer.Views;


namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for AttemptResultWindow.xaml
    /// </summary>
    public partial class AttemptResultWindow : Window
    {
        AttemptInfo? attemptInfo;
        AttemptServices services;
        AnswerService answerService;
        bool IsResultShowable;
        MenuWindow menu;
        int state = 0;
        public AttemptResultWindow(int attempID, int quizzID, MenuWindow menu, bool IsResultShowable)
        {
            InitializeComponent();
            services = new AttemptServices();
            answerService = new AnswerService();
            attemptInfo = services.GetAttemptInfoByID(attempID);
            this.menu = menu;
            this.IsResultShowable = IsResultShowable;
            // Bind dữ liệu vào Grid
            BindDataToGrid();
            LoadShowResultBtn();
            List<AnswerResult> results = LoadResult(attempID, quizzID);
            ShowResults(results);
        }
        public void ShowResults(List<AnswerResult> results)
        {
            ResultsPanel.Children.Clear();

            foreach (var result in results)
            {
                var questionPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 10, 0, 10)
                };

                var questionText = new TextBlock
                {
                    Text = result.Question,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                questionPanel.Children.Add(questionText);

                for (int i = 1; i <= 4; i++)
                {
                    var answerText = result.GetType().GetProperty($"Answer{i}")?.GetValue(result, null)?.ToString();
                    if (!string.IsNullOrEmpty(answerText))
                    {
                        var answerPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 5)
                        };

                        var radioButton = new RadioButton
                        {
                            Content = answerText,
                            Tag = i,
                            IsEnabled = false,
                            Margin = new Thickness(0, 5, 0, 5)
                        };

                        if (result.CorrectAnswer.ToString() == radioButton.Tag.ToString())
                        {
                            radioButton.IsChecked = true;

                            Brush backgroundColor = Brushes.White;

                            if (result.Answer != null)
                            {
                                if (result.CorrectAnswer.ToString() == result.Answer.ToString())
                                {
                                    backgroundColor = Brushes.LightGreen;
                                }
                                else
                                {
                                    backgroundColor = Brushes.LightCoral;
                                }
                            }
                            else
                            {
                                backgroundColor = Brushes.LightCoral;
                            }

                            answerPanel.Background = backgroundColor;
                        }

                        answerPanel.Children.Add(radioButton);

                        questionPanel.Children.Add(answerPanel);
                    }
                }

                ResultsPanel.Children.Add(questionPanel);
            }
        }


        private List<AnswerResult> LoadResult(int attempID, int quizzID)
        {
            List<AnswerResult> answerResults = this.answerService.GetAnswers(attempID, quizzID);
            return answerResults;
        }

        private void LoadShowResultBtn()
        {
            if (!this.IsResultShowable)
            {
                ShowResultBtn.Background = Brushes.Gray;
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.menu.Show();
        }

        private void ShowResultBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsResultShowable)
            {
                return;
            }

            if (this.state == 0)
            {
                ResultNotification.Visibility = Visibility.Hidden;
                ShowResultView.Visibility = Visibility.Visible;
                ShowResultBtn.Content = "Result";
                this.state = 1;
            }
            else
            {
                ResultNotification.Visibility = Visibility.Visible;
                ShowResultView.Visibility = Visibility.Hidden;
                this.state = 0;
                ShowResultBtn.Content = "Answer";

            }

        }

        private void ShowResultBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsResultShowable)
            {
                ShowResultBtn.Cursor = Cursors.Hand;
            }
            else
            {
                ShowResultBtn.Cursor = Cursors.No;
                ShowResultBtn.Background = Brushes.LightGray;
            }
        }

        private void ShowResultBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsResultShowable)
            {
                ShowResultBtn.Cursor = Cursors.Hand;
            }
            else
            {
                ShowResultBtn.Cursor = Cursors.No;
                ShowResultBtn.Background = Brushes.Gray;
            }
        }
        private void BindDataToGrid()
        {
            if (attemptInfo != null)
            {
                // Gán giá trị vào các TextBlock trong Grid
                AnswerBy.Text = attemptInfo.Username;
                QuizzName.Text = attemptInfo.Title;
                CorrectNumber.Text = attemptInfo.CorrectNumber.ToString();
                Time.Text = attemptInfo.Time.ToString();
                IsCompelete.Text = attemptInfo.Complete ? "Completed" : "Not Completed";
                StartAt.Text = attemptInfo.StartAt.ToString();
            }
            else
            {
                MessageBox.Show("Attempt data not found.");
            }
        }


    }

}
