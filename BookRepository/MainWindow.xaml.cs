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

namespace BookRepository
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Yeni pencere açma örneği:
            //this.Show();
            //LoginWindow loginWindow = new LoginWindow();
            //loginWindow.ShowDialog();

            var returned = SqlHandler.GetBook("fsfsgs");
            if (!returned.Success) MessageBox.Show(returned.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else MessageBox.Show("Got book: " + returned.Book.BookTitle);
        }
    }
}
