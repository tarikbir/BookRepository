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

namespace BookRepository.AdminPanel
{
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
        }

        private void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            BookListPanel BookPanel = new BookListPanel();
            BookPanel.ShowDialog();
        }

        private void btnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            UserListPanel UserPanel = new UserListPanel();
            UserPanel.ShowDialog();
        }

        private void btnUpdateWeights_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This progress will take a really long time (about 30 mins). If you abruptly close the process, it might corrupt some data on the database. " +
                "Do you want to continue anyway?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var response = SqlHandler.UpdateAllWeights();
                if (response.Success)
                {
                    MessageBox.Show("Database successfully updated.", "Success",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was an error updating the database.\n\n" + response.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
