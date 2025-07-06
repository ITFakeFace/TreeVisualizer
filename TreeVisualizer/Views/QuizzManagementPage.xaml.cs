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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeVisualizer.Models;
using TreeVisualizer.Services;
using TreeVisualizer.Views;

namespace TreeVisualizer.Views
{
    public partial class QuizzManagementPage : Page
    {
        private QuizzService _quizzService = new QuizzService();

        private List<string> _topicList = new List<string> { "English" };

        public QuizzManagementPage()
        {
            InitializeComponent();

            // init data of topic
            foreach (string topic in _topicList)
                CBoxTopic.Items.Add(topic);

            UpdateListBoxQuestion();
        }

        public void UpdateListBoxQuestion()
        {
            var questionList = _quizzService.GetByUserId(MenuWindow.UserId);
            ListBoxQuestion.ItemsSource = questionList;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (InpTitle.Text == string.Empty)
            {
                LblTitle.Foreground = Brushes.Red;
                InpTitle.Foreground = Brushes.Red;
                InpTitle.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter question title", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTitle.Foreground = Brushes.Black;
            InpTitle.Foreground = Brushes.Black;
            InpTitle.BorderBrush = Brushes.Black;

            if (CBoxTopic.SelectedIndex == -1)
            {
                LblTopic.Foreground = Brushes.Red;
                CBoxTopic.Foreground = Brushes.Red;
                CBoxTopic.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please choose question topic", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTopic.Foreground = Brushes.Black;
            CBoxTopic.Foreground = Brushes.Black;
            CBoxTopic.BorderBrush = Brushes.Black;

            if (InpTimeLimit.Value == null)
            {
                LblTimeLimit.Foreground = Brushes.Red;
                InpTimeLimit.Foreground = Brushes.Red;
                InpTimeLimit.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter time limit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTimeLimit.Foreground = Brushes.Black;
            InpTimeLimit.Foreground = Brushes.Black;
            InpTimeLimit.BorderBrush = Brushes.Black;

            if (InpAttempNumber.Text != string.Empty && !ValidationHelper.ValidateUnsignedNumberFormat(InpAttempNumber.Text))
            {
                LblAttempNumber.Foreground = Brushes.Red;
                InpAttempNumber.Foreground = Brushes.Red;
                InpAttempNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter valid attemp number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblAttempNumber.Foreground = Brushes.Black;
            InpAttempNumber.Foreground = Brushes.Black;
            InpAttempNumber.BorderBrush = Brushes.Black;

            if (InpStartAt.Value >= InpEndAt.Value)
            {
                MessageBox.Show("Error: Start Time must be before end time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var quizz = new Quizz
            {
                Title = InpTitle.Text.Trim(),
                Type = CBoxTopic.Text,
                TimeLimit = InpTimeLimit.Value.Value.TimeOfDay,
                AttempNumber = InpAttempNumber.Text == string.Empty ? null : int.Parse(InpAttempNumber.Text.Trim()),
                IsRandom = (bool)CheckBoxRandomQuestion.IsChecked,
                IsResultShowable = (bool)CheckBoxShowResult.IsChecked,
                StartAt = InpStartAt.Value,
                EndAt = InpEndAt.Value,
                CreatedBy = MenuWindow.UserId,
            };

            if (_quizzService.Create(quizz))
            {
                MessageBox.Show("Message: Successfully created new quizz", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateListBoxQuestion();
            }
            else
            {
                MessageBox.Show("Error: Cannot create quizz", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ListBoxQuestion.UnselectAll();
        }
        private void ListBoxQuestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxQuestion.SelectedValue == null)
                return;
            var quizz = (Quizz)ListBoxQuestion.SelectedValue;
            InpTitle.Text = quizz.Title;
            CBoxTopic.Text = quizz.Type;
            InpTimeLimit.Value = quizz.TimeLimit.HasValue
            ? DateTime.Today + quizz.TimeLimit.Value
            : null;
            InpAttempNumber.Text = quizz.AttempNumber + "";
            CheckBoxRandomQuestion.IsChecked = quizz.IsRandom;
            CheckBoxShowResult.IsChecked = quizz.IsResultShowable;
            InpStartAt.Value = quizz.StartAt;
            InpEndAt.Value = quizz.EndAt;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (InpTitle.Text == string.Empty)
            {
                LblTitle.Foreground = Brushes.Red;
                InpTitle.Foreground = Brushes.Red;
                InpTitle.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter question title", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTitle.Foreground = Brushes.Black;
            InpTitle.Foreground = Brushes.Black;
            InpTitle.BorderBrush = Brushes.Black;

            if (CBoxTopic.SelectedIndex == -1)
            {
                LblTopic.Foreground = Brushes.Red;
                CBoxTopic.Foreground = Brushes.Red;
                CBoxTopic.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please choose question topic", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTopic.Foreground = Brushes.Black;
            CBoxTopic.Foreground = Brushes.Black;
            CBoxTopic.BorderBrush = Brushes.Black;

            if (InpTimeLimit.Value == null)
            {
                LblTimeLimit.Foreground = Brushes.Red;
                InpTimeLimit.Foreground = Brushes.Red;
                InpTimeLimit.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter time limit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblTimeLimit.Foreground = Brushes.Black;
            InpTimeLimit.Foreground = Brushes.Black;
            InpTimeLimit.BorderBrush = Brushes.Black;

            if (InpAttempNumber.Text != string.Empty && !ValidationHelper.ValidateUnsignedNumberFormat(InpAttempNumber.Text))
            {
                LblAttempNumber.Foreground = Brushes.Red;
                InpAttempNumber.Foreground = Brushes.Red;
                InpAttempNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Error: Please enter valid attemp number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LblAttempNumber.Foreground = Brushes.Black;
            InpAttempNumber.Foreground = Brushes.Black;
            InpAttempNumber.BorderBrush = Brushes.Black;
            if (ListBoxQuestion.SelectedValue == null)
            {
                MessageBox.Show("Error: Please choose quizz to edit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var quizz = (Quizz)ListBoxQuestion.SelectedValue;
            quizz.Title = InpTitle.Text.Trim();
            quizz.Type = CBoxTopic.Text;
            quizz.TimeLimit = InpTimeLimit.Value.Value.TimeOfDay;
            quizz.AttempNumber = InpAttempNumber.Text == string.Empty ? null : int.Parse(InpAttempNumber.Text.Trim());
            quizz.IsRandom = (bool)CheckBoxRandomQuestion.IsChecked;
            quizz.IsResultShowable = (bool)CheckBoxShowResult.IsChecked;
            quizz.StartAt = InpStartAt.Value;
            quizz.EndAt = InpEndAt.Value;
            _quizzService.Update(quizz);
            MessageBox.Show("Message: Successfully update quizz", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateListBoxQuestion();
            ListBoxQuestion.UnselectAll();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxQuestion.SelectedValue == null)
            {
                MessageBox.Show("Error: Please choose quizz", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var quizz = (Quizz)ListBoxQuestion.SelectedValue;
            var window = Window.GetWindow(this) as MenuWindow;
            window.MainFrame.Navigate(new QuizzDetailsPage(quizz.Id));
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxQuestion.SelectedValue == null)
            {
                MessageBox.Show("Error: Please choose quizz", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var quizz = (Quizz)ListBoxQuestion.SelectedValue;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the quiz: \"{quizz.Title}\"?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                _quizzService.Delete(quizz.Id);
                MessageBox.Show("Quiz deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                ListBoxQuestion.UnselectAll();
            }
        }
    }
}
