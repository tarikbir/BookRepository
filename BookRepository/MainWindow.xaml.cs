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
            //LoginWindow loginWindow = new LoginWindow();
            //loginWindow.ShowDialog();

            //var returned = SqlHandler.GetBook("0001010565");
            //if (!returned.Success) MessageBox.Show(returned.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //else MessageBox.Show("Got book: " + returned.Book.BookTitle);

            //wrapNews.Children.Add(new BookImage(returned.Book));
            wrapNews.Children.Add(new BookImage(SqlHandler.GetBook("0001046934").Book));
            wrapNews.Children.Add(new BookImage(SqlHandler.GetBook("0001047868").Book));

            wrapRecommended.Children.Add(new BookImage(SqlHandler.GetBook("000104799X").Book));
            wrapRecommended.Children.Add(new BookImage(SqlHandler.GetBook("0001048473").Book));

            wrapMostLiked.Children.Add(new BookImage(SqlHandler.GetBook("0001053736").Book));
            wrapMostLiked.Children.Add(new BookImage(SqlHandler.GetBook("0001056107").Book));


        }
    }
}
