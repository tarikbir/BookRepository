using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public string UserName;

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
            LoadNews();
            LoadMostLiked();
            LoadRecommended();

        }

        private void LoadRecommended()
        {
            // User-Based Collaborative Filtering Algorithm
            //await Task.Run<int> (() => wrapRecommended.Children.Add(new BookObject(SqlHandler.GetBook("000104799X").Book)));
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    wrapRecommended.Children.Add(new BookObject(SqlHandler.GetBook(item).Book));
                }
            });
        }

        private void LoadMostLiked()
        {
            //SQL Highest Book Rating Return
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(()=>
            {
                foreach(string item in list)
                {
                    wrapMostLiked.Children.Add(new BookObject(SqlHandler.GetBook(item).Book));
                }
            });
            /*
            foreach (string item in list)
            {
                await Task.Run<int>(() => wrapRecommended.Children.Add(new BookObject(SqlHandler.GetBook(item).Book)));
            }*/
            
        }

        private void LoadNews()
        {
            //SQL Latest Book Additions Return
            //await Task.Run<int>(() => wrapRecommended.Children.Add(new BookObject(SqlHandler.GetBook("000104799X").Book)));
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    wrapNews.Children.Add(new BookObject(SqlHandler.GetBook(item).Book));
                }
            });
        }
    }
}
