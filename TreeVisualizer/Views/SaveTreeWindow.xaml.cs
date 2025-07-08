using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
using TreeVisualizer.Components.Algorithm;
using TreeVisualizer.Services;

namespace TreeVisualizer.Views
{
    /// <summary>
    /// Interaction logic for SaveTreeWindow.xaml
    /// </summary>
    public partial class SaveTreeWindow : Window
    {
        public string TreeName { get; private set; }
        public string TreeDescription { get; private set; }
        NodeUserControl root;
        string treeType;
        TreeService _treeService = new TreeService();
        public SaveTreeWindow(NodeUserControl root, string treeType)
        {
            this.root = root;
            this.treeType = treeType;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TreeName = TreeNameBox.Text.Trim();
            TreeDescription = TreeDescriptionBox.Text.Trim();

            if (string.IsNullOrEmpty(TreeName))
            {
                MessageBox.Show("Name cannot be empty", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var response = _treeService.CreateOrUpdate(new DTOS.TreeCreateDTO
            {
                Name = TreeName,
                Description = TreeDescription,
                TreeType = this.treeType
            }, this.root, null);

            if (response.Status == false)
            {
                new ErrorMessageWindow(response.StatusMessage ?? "Save failed").ShowDialog();
                return;
            }

            var successWindow = new SuccessMessageWindow("Tree saved successfully.");
            successWindow.Owner = this; // Gắn owner nếu muốn modal
            successWindow.ShowDialog();

            this.DialogResult = true;
            this.Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // giống WinForms: DialogResult.Cancel
            this.Close();
        }
    }
}
