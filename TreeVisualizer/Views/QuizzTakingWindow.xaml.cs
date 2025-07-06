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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TreeVisualizer.Models;
using TreeVisualizer.Services;
using TreeVisualizer.Views;

namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for QuizzTakingWindows.xaml
    /// </summary>
    public partial class QuizzTakingWindow : Window
    {
        QuizzDetailsService _quizzDetailsService;
        AttempServices _attempServices;
        AnswerService _answerService;
        List<QuizzDetails> quizzDetails;
        DateTime currentDateTime;

        public Dictionary<int, AnswerDTO> _quizzAnswerDict = new Dictionary<int, AnswerDTO>();
        private int currentQuestion = 0;
        private DispatcherTimer countdownTimer;
        private int remainingTimeInSeconds;
        private int quizzID;
        private int timeLimitInSeconds;
        private MenuWindow _menuWindow;
        private bool IsResultShowable;

        public QuizzTakingWindow(int quizzID, MenuWindow menu, int timeLimit, bool IsResultShowable)
        {
            InitializeComponent();
            this._menuWindow = menu;
            this._quizzDetailsService = new QuizzDetailsService();
            this.currentDateTime = DateTime.Now;
            this.quizzID = quizzID;
            this.timeLimitInSeconds = timeLimit * 60;
            this.remainingTimeInSeconds = timeLimit * 60;
            this.IsResultShowable = IsResultShowable;
            this.quizzDetails = _quizzDetailsService.GetByQuizzId(quizzID);
            LoadQuestion(currentQuestion);
            InitializeQuestionChoice(quizzDetails.Count());
            StartCountdown();
        }

        private void StartCountdown()
        {
            // Tạo một Timer
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1); // Cập nhật mỗi giây
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (remainingTimeInSeconds > 0)
            {
                remainingTimeInSeconds--;
                // Hiển thị thời gian còn lại dưới định dạng phút:giây
                int minutes = remainingTimeInSeconds / 60;
                int seconds = remainingTimeInSeconds % 60;
                CountdownTextBlock.Text = $"{minutes:D2}:{seconds:D2}";
            }
            else
            {
                countdownTimer.Stop(); // Dừng timer khi hết thời gian
                //SubmitQuiz(); // Gọi hàm nộp bài khi hết thời gian
            }
        }


        private void QuestionChoice_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedBtn)
            {
                int questionNumber = int.Parse(clickedBtn.Content.ToString()!) - 1;
                this.currentQuestion = questionNumber;
                LoadQuestion(questionNumber);
                if (questionNumber == quizzDetails.Count() - 1)
                {
                    NextBtn.Visibility = Visibility.Hidden;
                    SubmitBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    NextBtn.Visibility = Visibility.Visible;
                    SubmitBtn.Visibility = Visibility.Hidden;
                }
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển sang câu hỏi tiếp theo
            if (this.currentQuestion + 1 < quizzDetails.Count)
            {
                this.currentQuestion += 1;
                // Gọi hàm LoadQuestion để load câu hỏi tiếp theo
                LoadQuestion(this.currentQuestion);
            }
            else
            {
                NextBtn.Visibility = Visibility.Collapsed;
                SubmitBtn.Visibility = Visibility.Visible;
            }
        }

        private void SubmitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _attempServices = new AttempServices();
            _answerService = new AnswerService();

            List<AnswerDTO> answerList = this._quizzAnswerDict.Values.ToList();
            bool isCompleted = answerList.Count() == this.quizzDetails.Count();
            int currentTime = this.timeLimitInSeconds - this.remainingTimeInSeconds;
            Attempt attemp = CreateModelObj.CreateAttemp(answerList, this.quizzID, currentTime, isCompleted, this.currentDateTime);
            int attemptID = _attempServices.Create(attemp);

            List<Answer> answers = CreateModelObj.CreateAnswers(answerList, attemptID);
            _answerService.CreateAnswers(answers);

            Window window = new AttemptResultWindow(attemptID, this.quizzID, _menuWindow, this.IsResultShowable);
            window.Show();
            this.Close();
        }
        // Hàm chức năng
        private void InitializeQuestionChoice(int quizzDetailCount)
        {
            for (int i = 0; i < quizzDetails.Count(); i++)
            {
                var btn = new Button
                {
                    Content = (i + 1).ToString(),
                    Tag = $"Câu {i + 1}",
                    Style = (Style)FindResource("QuestionChoiceBtn"),
                    Margin = new Thickness(5)
                };

                btn.Click += QuestionChoice_Click;

                QuestionChoicePanel.Children.Add(btn);
            }
        }
        private void SaveAnswerToDictionary(int questionNumber, int selectedAnswer)
        {
            if (_quizzAnswerDict.ContainsKey(questionNumber))
            {
                _quizzAnswerDict[questionNumber].Answer = selectedAnswer;
            }
            else
            {
                _quizzAnswerDict.Add(questionNumber, new AnswerDTO(quizzDetails[questionNumber].Id, selectedAnswer, quizzDetails[questionNumber].CorrectAnswer));
            }
        }
        private void Answer_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton selectedRadioButton)
            {

                int selectedAnswer = (int)selectedRadioButton.Tag;
                SaveAnswerToDictionary(this.currentQuestion, selectedAnswer);

                var questionButton = QuestionChoicePanel.Children[this.currentQuestion] as Button;
                if (questionButton != null) { questionButton.Background = Brushes.Green; }
            }
        }
        private void LoadQuestion(int questionNumber)
        {
            // Kiểm tra câu hỏi có hợp lệ không
            if (questionNumber >= 0 && questionNumber < quizzDetails.Count)
            {
                var currentQuestion = quizzDetails[questionNumber];
                questionTxtBlock.Text = currentQuestion.Question;

                // Cập nhật nội dung cho các RadioButton
                Q1.Content = currentQuestion.Answer1; Q1.Tag = 1; Q1.IsChecked = false;
                Q2.Content = currentQuestion.Answer2; Q2.Tag = 2; Q2.IsChecked = false;
                Q3.Content = currentQuestion.Answer3; Q3.Tag = 3; Q3.IsChecked = false;
                Q4.Content = currentQuestion.Answer4; Q4.Tag = 4; Q4.IsChecked = false;

                // Kiểm tra và đánh dấu đáp án đã chọn (nếu có)
                if (_quizzAnswerDict.ContainsKey(questionNumber))
                {
                    int selectedAnswer = _quizzAnswerDict[questionNumber].Answer;
                    switch (selectedAnswer)
                    {
                        case 1: Q1.IsChecked = true; break;
                        case 2: Q2.IsChecked = true; break;
                        case 3: Q3.IsChecked = true; break;
                        case 4: Q4.IsChecked = true; break;
                    }
                }
            }
        }


    }
}
