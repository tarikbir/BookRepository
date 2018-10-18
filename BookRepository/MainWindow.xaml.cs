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
        //public User currentUser;
        private BackgroundWorker bgwNews;
        private BackgroundWorker bgwPopular;
        private BackgroundWorker bgwRecommended;

        public MainWindow()
        {
            InitializeComponent();
            bgwNews = new BackgroundWorker();
            bgwPopular = new BackgroundWorker();
            bgwRecommended = new BackgroundWorker();
            LoginWindow loginWindow = new LoginWindow();

            if (!CheckConnection()) this.Close();

            InitializeBackgroundWorkers();
            loginWindow.ShowDialog();
            if (loginWindow.login?.Success == true)
            {
                lblGreeting.Content = $"Welcome {loginWindow.login.Username},";
                bgwNews.RunWorkerAsync();
                bgwPopular.RunWorkerAsync();
                bgwRecommended.RunWorkerAsync();
            }
        }

        private void InitializeBackgroundWorkers()
        {
            bgwNews.DoWork += new DoWorkEventHandler(bgwNews_DoWork);
            bgwPopular.DoWork += new DoWorkEventHandler(bgwPopular_DoWork);
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

        private void bgwPopular_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Highest Book Rating Return
            //SELECT N.`ISBN`, (NumberRating/1147285)*AverageRatingForItem+(1-(NumberRating/1147285))*2.866898 AS Weight FROM `bx-book-ratings` AS B INNER JOIN (SELECT `ISBN`, COUNT(`ISBN`) AS NumberRating, AVG(`Book-Rating`) AS AverageRatingForItem FROM `bx-book-ratings` GROUP BY `ISBN`) AS N on N.`ISBN` = B.`ISBN` GROUP BY N.`ISBN` ORDER BY Weight DESC LIMIT 10
            List<string> list = new List<string>() { "0316666343", "0385504209", "059035342X", "0312195516", "0679781587", "043935806X", "0142001740", "0446310786", "0446672211", "0786868716" };
            this.Dispatcher.Invoke(() =>
            {
                foreach (string item in list)
                {
                    Book book = SqlHandler.GetBook(item).Book;
                    if (book != null)
                        wrapPopular.Children.Add(new BookObject(book));
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

        private bool CheckConnection()
        {
            while (!SqlHandler.IsConnected())
            {
                if (MessageBox.Show("Error connecting to the server. Would you like to retry?", "Connection Error",
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
