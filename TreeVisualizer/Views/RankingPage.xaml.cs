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
using static System.Net.Mime.MediaTypeNames;

namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for RankingPage.xaml
    /// </summary>
    public partial class RankingPage : Page
    {
        public int QuizzId { get; set; }
        private readonly AttempServices _attempServices = new AttempServices();
        private readonly UserService _userServices = new UserService();
        private readonly QuizzDetailsService _quizzDetailsServices = new QuizzDetailsService();
        public RankingPage(int quizzId)
        {
            InitializeComponent();
            QuizzId = quizzId;
            UpdateRow();
        }

        public void UpdateRow()
        {
            List<Attempt> attemps = new List<Attempt>();
            attemps = _attempServices.GetByQuizzId(QuizzId);
            // Clear tất cả Row cũ (trừ tiêu đề)
            while (RankingTableGrid.RowDefinitions.Count > 2)
            {
                RankingTableGrid.RowDefinitions.RemoveAt(RankingTableGrid.RowDefinitions.Count - 1);
            }
            RankingTableGrid.Children.RemoveRange(4, RankingTableGrid.Children.Count - 4); // Giữ lại tiêu đề

            if (attemps.Count == 0)
            {
                int newRowIndex = RankingTableGrid.RowDefinitions.Count;
                RankingTableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var border = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5)
                };

                var textBlock = new TextBlock
                {
                    Text = "No records",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10)
                };

                border.Child = textBlock;

                Grid.SetRow(border, newRowIndex);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 4); // Chiếm 4 cột

                RankingTableGrid.Children.Add(border);
            }
            else
            {
                // Load dữ liệu thật
                int rank = 1;
                List<RankingModel> listRanking = new List<RankingModel>();
                foreach (var item in attemps)
                {
                    var number = _attempServices.GetTotalQuizzDetails(item.AnsweredBy, QuizzId);
                    listRanking.Add(new RankingModel
                    {
                        Id = item.AnsweredBy,
                        Username = _userServices.GetById(item.AnsweredBy).Username,
                        Score = (float)(item.CorrectNumber / (int)_quizzDetailsServices.GetTotalQuizzDetails(item.QuizzId)),
                        Time = item.Time,
                    });
                }
                for (int i = 0; i < listRanking.Count - 1; i++)
                    for (int j = 1; j < listRanking.Count; j++)
                        if (listRanking[i].Score < listRanking[j].Score)
                        {
                            var temp = listRanking[i];
                            listRanking[i] = listRanking[j];
                            listRanking[j] = temp;
                        }
                        else if (listRanking[i].Score == listRanking[j].Score && listRanking[i].Time > listRanking[j].Time)
                        {
                            var temp = listRanking[i];
                            listRanking[i] = listRanking[j];
                            listRanking[j] = temp;
                        }
                foreach (var item in listRanking)
                {
                    AddRankingRow(rank++, item.Username, item.Score, item.Time);
                }
            }
        }

        public void AddRankingRow(int rank, string username, float score, TimeSpan time)
        {
            int newRowIndex = RankingTableGrid.RowDefinitions.Count;

            // Thêm hàng mới
            RankingTableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Tạo từng ô trong hàng
            AddCellToGrid(RankingTableGrid, newRowIndex, 0, rank.ToString());
            AddCellToGrid(RankingTableGrid, newRowIndex, 1, username);
            AddCellToGrid(RankingTableGrid, newRowIndex, 2, score.ToString());
            AddCellToGrid(RankingTableGrid, newRowIndex, 3, time.ToString());
        }

        private void AddCellToGrid(Grid grid, int row, int column, string text)
        {
            var border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            border.Child = textBlock;

            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            grid.Children.Add(border);
        }
    }
}
