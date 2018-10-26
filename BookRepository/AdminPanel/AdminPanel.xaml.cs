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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
