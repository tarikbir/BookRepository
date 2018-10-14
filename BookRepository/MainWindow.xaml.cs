using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BackgroundWorker bgwNews;
        private BackgroundWorker bgwMostLiked;
        private BackgroundWorker bgwRecommended;

        public MainWindow()
        {
            InitializeComponent();
            bgwNews = new BackgroundWorker();
            bgwMostLiked = new BackgroundWorker();
            bgwRecommended = new BackgroundWorker();
            InitializeBackgroundWorkers();
            LoginWindow loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                bgwNews.RunWorkerAsync();
                bgwMostLiked.RunWorkerAsync();
                bgwRecommended.RunWorkerAsync();
            }
            else
            {
                this.Close();
            }
        }

        private void InitializeBackgroundWorkers()
        {
            bgwNews.DoWork += new DoWorkEventHandler(bgwNews_DoWork);
            bgwMostLiked.DoWork += new DoWorkEventHandler(bgwMostLiked_DoWork);
            bgwRecommended.DoWork += new DoWorkEventHandler(bgwRecommended_DoWork);
        }

        private void bgwNews_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Latest Book Additions Return
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    Book book = SqlHandler.GetBook(item).Book;
                    if (book != null)
                        wrapNews.Children.Add(new BookObject(book));
                }
            });
        }

        private void bgwMostLiked_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Highest Book Rating Return
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    Book book = SqlHandler.GetBook(item).Book;
                    if (book != null)
                        wrapMostLiked.Children.Add(new BookObject(book));
                }
            });
        }

        private void bgwRecommended_DoWork(object sender, DoWorkEventArgs e)
        {
            // User-Based Collaborative Filtering Algorithm
            List<string> list = new List<string>() { "000104799X", "0001046713", "0001046934", "0001047663", "000104799X", "0001061127", "0001053736" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    Book book = SqlHandler.GetBook(item).Book;
                    if (book != null)
                        wrapRecommended.Children.Add(new BookObject(book));
                }
            });
        }
    }
}
