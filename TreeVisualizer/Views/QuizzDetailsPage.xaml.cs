using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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

namespace TreeVisualizer.Views
{
    public partial class QuizzDetailsPage : Page
    {
        private int QuizzID;
        private QuizzDetailsService _quizzDetailsService = new QuizzDetailsService();
        public QuizzDetailsPage(int quizzID)
        {
            InitializeComponent();
            QuizzID = quizzID;
            UpdateListBoxDetails();
        }

        public void UpdateListBoxDetails()
        {
            var questionList = _quizzDetailsService.GetByQuizzId(QuizzID);
            ListBoxDetails.ItemsSource = questionList;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (InpQuestion.Text == string.Empty)
            {
                MessageBox.Show("Error: Please input question", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if ((InpAnswer1.Text == string.Empty && (bool)RadioAnswer1.IsChecked)
            || (InpAnswer2.Text == string.Empty && (bool)RadioAnswer2.IsChecked)
            || (InpAnswer3.Text == string.Empty && (bool)RadioAnswer3.IsChecked)
            || (InpAnswer4.Text == string.Empty && (bool)RadioAnswer4.IsChecked))
            {
                MessageBox.Show("Error: The Correct Answer cannot be empty answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (InpAnswer1.Text == string.Empty &&
            InpAnswer2.Text == string.Empty &&
            InpAnswer3.Text == string.Empty &&
            InpAnswer4.Text == string.Empty)
            {
                MessageBox.Show("Error: Please input answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!(bool)RadioAnswer1.IsChecked &&
            !(bool)RadioAnswer2.IsChecked &&
            !(bool)RadioAnswer3.IsChecked &&
            !(bool)RadioAnswer4.IsChecked)
            {
                MessageBox.Show("Error: Please choose correct answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var answers = new List<string>
    {
    InpAnswer1.Text.Trim(),
    InpAnswer2.Text.Trim(),
    InpAnswer3.Text.Trim(),
    InpAnswer4.Text.Trim()
    };

            var radios = new List<bool?>
    {
    RadioAnswer1.IsChecked,
    RadioAnswer2.IsChecked,
    RadioAnswer3.IsChecked,
    RadioAnswer4.IsChecked
    };

            // Gom lại các answer không rỗng
            var nonEmptyAnswers = new List<(string Text, bool? IsChecked)>();
            for (int i = 0; i < answers.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(answers[i]))
                {
                    nonEmptyAnswers.Add((answers[i], radios[i]));
                }
            }

            // Đổ lại từ đầu, các vị trí trống được set ""
            InpAnswer1.Text = nonEmptyAnswers.Count > 0 ? nonEmptyAnswers[0].Text : "";
            RadioAnswer1.IsChecked = nonEmptyAnswers.Count > 0 ? nonEmptyAnswers[0].IsChecked : false;

            InpAnswer2.Text = nonEmptyAnswers.Count > 1 ? nonEmptyAnswers[1].Text : "";
            RadioAnswer2.IsChecked = nonEmptyAnswers.Count > 1 ? nonEmptyAnswers[1].IsChecked : false;

            InpAnswer3.Text = nonEmptyAnswers.Count > 2 ? nonEmptyAnswers[2].Text : "";
            RadioAnswer3.IsChecked = nonEmptyAnswers.Count > 2 ? nonEmptyAnswers[2].IsChecked : false;

            InpAnswer4.Text = nonEmptyAnswers.Count > 3 ? nonEmptyAnswers[3].Text : "";
            RadioAnswer4.IsChecked = nonEmptyAnswers.Count > 3 ? nonEmptyAnswers[3].IsChecked : false;

            QuizzDetails quizzDetails = new QuizzDetails();
            quizzDetails.QuizzId = QuizzID;
            quizzDetails.Question = InpQuestion.Text;
            quizzDetails.Answer1 = InpAnswer1.Text;
            quizzDetails.Answer2 = InpAnswer2.Text;
            quizzDetails.Answer3 = InpAnswer3.Text;
            quizzDetails.Answer4 = InpAnswer4.Text;
            if ((bool)RadioAnswer1.IsChecked)
                quizzDetails.CorrectAnswer = 1;
            else if ((bool)RadioAnswer2.IsChecked)
                quizzDetails.CorrectAnswer = 2;
            else if ((bool)RadioAnswer3.IsChecked)
                quizzDetails.CorrectAnswer = 3;
            else
                quizzDetails.CorrectAnswer = 4;
            if (_quizzDetailsService.Create(quizzDetails))
            {
                MessageBox.Show("Success: Sucessfully create new question", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error: Cannot create quizz details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ListBoxDetails.UnselectAll();
            UpdateListBoxDetails();
        }

        public void SwapAnswer(int src, int dest)
        {
            string answer = string.Empty;
            bool isChecked = false;
            switch (src)
            {
                case 1:
                    if ((bool)RadioAnswer1.IsChecked)
                    {
                        isChecked = true;
                        RadioAnswer1.IsChecked = false;
                    }
                    answer = InpAnswer1.Text;
                    break;
                case 2:
                    if ((bool)RadioAnswer2.IsChecked)
                    {
                        isChecked = true;
                        RadioAnswer2.IsChecked = false;
                    }
                    answer = InpAnswer2.Text;
                    break;
                case 3:
                    if ((bool)RadioAnswer3.IsChecked)
                    {
                        isChecked = true;
                        RadioAnswer3.IsChecked = false;
                    }
                    answer = InpAnswer3.Text;
                    break;
                case 4:
                    if ((bool)RadioAnswer4.IsChecked)
                    {
                        isChecked = true;
                        RadioAnswer4.IsChecked = false;
                    }
                    answer = InpAnswer4.Text;
                    break;
                default:
                    break;
            }
            string answerTemp = string.Empty;
            bool isCheckedTemp = false;
            switch (dest)
            {
                case 1:
                    if ((bool)RadioAnswer1.IsChecked)
                    {
                        isCheckedTemp = true;
                    }
                    answerTemp = InpAnswer1.Text;
                    RadioAnswer1.IsChecked = isChecked;
                    InpAnswer1.Text = answer;
                    break;
                case 2:
                    if ((bool)RadioAnswer2.IsChecked)
                    {
                        isCheckedTemp = true;
                    }
                    answerTemp = InpAnswer2.Text;
                    RadioAnswer2.IsChecked = isChecked;
                    InpAnswer2.Text = answer;
                    break;
                case 3:
                    if ((bool)RadioAnswer3.IsChecked)
                    {
                        isCheckedTemp = true;
                    }
                    answerTemp = InpAnswer3.Text;
                    RadioAnswer3.IsChecked = isChecked;
                    InpAnswer3.Text = answer;
                    break;
                case 4:
                    if ((bool)RadioAnswer4.IsChecked)
                    {
                        isCheckedTemp = true;
                    }
                    answerTemp = InpAnswer4.Text;
                    RadioAnswer4.IsChecked = isChecked;
                    InpAnswer4.Text = answer;
                    break;
                default:
                    break;
            }

            switch (src)
            {
                case 1:
                    RadioAnswer1.IsChecked = isCheckedTemp;
                    InpAnswer1.Text = answerTemp;
                    break;
                case 2:
                    RadioAnswer2.IsChecked = isCheckedTemp;
                    InpAnswer2.Text = answerTemp;
                    break;
                case 3:
                    RadioAnswer3.IsChecked = isCheckedTemp;
                    InpAnswer3.Text = answerTemp;
                    break;
                case 4:
                    RadioAnswer4.IsChecked = isCheckedTemp;
                    InpAnswer4.Text = answerTemp;
                    break;
                default:
                    break;
            }
        }

        private void ListBoxDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxDetails.SelectedValue == null)
            {
                return;
            }

            var quizzDetails = (QuizzDetails)ListBoxDetails.SelectedValue;
            InpQuestion.Text = quizzDetails.Question;
            InpAnswer1.Text = quizzDetails.Answer1;
            InpAnswer2.Text = quizzDetails.Answer2;
            InpAnswer3.Text = quizzDetails.Answer3;
            InpAnswer4.Text = quizzDetails.Answer4;

            RadioAnswer1.IsChecked = false;
            RadioAnswer2.IsChecked = false;
            RadioAnswer3.IsChecked = false;
            RadioAnswer4.IsChecked = false;
            switch (quizzDetails.CorrectAnswer)
            {
                case 1:
                    RadioAnswer1.IsChecked = true;
                    break;
                case 2:
                    RadioAnswer2.IsChecked = true;
                    break;
                case 3:
                    RadioAnswer3.IsChecked = true;
                    break;
                case 4:
                    RadioAnswer4.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxDetails.SelectedValue == null)
            {
                MessageBox.Show("Error: Please choose question", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (InpQuestion.Text == string.Empty)
            {
                MessageBox.Show("Error: Please input question", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if ((InpAnswer1.Text == string.Empty && (bool)RadioAnswer1.IsChecked)
            || (InpAnswer2.Text == string.Empty && (bool)RadioAnswer2.IsChecked)
            || (InpAnswer3.Text == string.Empty && (bool)RadioAnswer3.IsChecked)
            || (InpAnswer4.Text == string.Empty && (bool)RadioAnswer4.IsChecked))
            {
                MessageBox.Show("Error: The Correct Answer cannot be empty answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (InpAnswer1.Text == string.Empty &&
            InpAnswer2.Text == string.Empty &&
            InpAnswer3.Text == string.Empty &&
            InpAnswer4.Text == string.Empty)
            {
                MessageBox.Show("Error: Please input answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!(bool)RadioAnswer1.IsChecked &&
            !(bool)RadioAnswer2.IsChecked &&
            !(bool)RadioAnswer3.IsChecked &&
            !(bool)RadioAnswer4.IsChecked)
            {
                MessageBox.Show("Error: Please choose correct answer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("After Validation", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var answers = new List<string>
                                            {
                                            InpAnswer1.Text.Trim(),
                                            InpAnswer2.Text.Trim(),
                                            InpAnswer3.Text.Trim(),
                                            InpAnswer4.Text.Trim()
                                            };

            var radios = new List<bool?>
                                            {
                                            RadioAnswer1.IsChecked,
                                            RadioAnswer2.IsChecked,
                                            RadioAnswer3.IsChecked,
                                            RadioAnswer4.IsChecked
                                            };

            // Gom lại các answer không rỗng
            var nonEmptyAnswers = new List<(string Text, bool? IsChecked)>();
            for (int i = 0; i < answers.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(answers[i]))
                {
                    nonEmptyAnswers.Add((answers[i], radios[i]));
                }
            }


            // Đổ lại từ đầu, các vị trí trống được set ""
            InpAnswer1.Text = nonEmptyAnswers.Count > 0 ? nonEmptyAnswers[0].Text : "";
            RadioAnswer1.IsChecked = nonEmptyAnswers.Count > 0 ? nonEmptyAnswers[0].IsChecked : false;

            InpAnswer2.Text = nonEmptyAnswers.Count > 1 ? nonEmptyAnswers[1].Text : "";
            RadioAnswer2.IsChecked = nonEmptyAnswers.Count > 1 ? nonEmptyAnswers[1].IsChecked : false;

            InpAnswer3.Text = nonEmptyAnswers.Count > 2 ? nonEmptyAnswers[2].Text : "";
            RadioAnswer3.IsChecked = nonEmptyAnswers.Count > 2 ? nonEmptyAnswers[2].IsChecked : false;

            InpAnswer4.Text = nonEmptyAnswers.Count > 3 ? nonEmptyAnswers[3].Text : "";
            RadioAnswer4.IsChecked = nonEmptyAnswers.Count > 3 ? nonEmptyAnswers[3].IsChecked : false;

            var quizzDetails = (QuizzDetails)ListBoxDetails.SelectedValue;

            quizzDetails.QuizzId = QuizzID;
            quizzDetails.Question = InpQuestion.Text;
            quizzDetails.Answer1 = InpAnswer1.Text;
            quizzDetails.Answer2 = InpAnswer2.Text;
            quizzDetails.Answer3 = InpAnswer3.Text;
            quizzDetails.Answer4 = InpAnswer4.Text;
            if ((bool)RadioAnswer1.IsChecked)
                quizzDetails.CorrectAnswer = 1;
            else if ((bool)RadioAnswer2.IsChecked)
                quizzDetails.CorrectAnswer = 2;
            else if ((bool)RadioAnswer3.IsChecked)
                quizzDetails.CorrectAnswer = 3;
            else
                quizzDetails.CorrectAnswer = 4;


            _quizzDetailsService.Update(quizzDetails);
            MessageBox.Show("Success: Sucessfully create new question", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateListBoxDetails();
            ListBoxDetails.UnselectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxDetails.SelectedValue == null)
            {
                return;
            }
            var quizzDetails = (QuizzDetails)ListBoxDetails.SelectedValue;
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the quiz: \"{quizzDetails.Question}\"?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                _quizzDetailsService.Delete(quizzDetails.Id);
                MessageBox.Show("Quiz deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
