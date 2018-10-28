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
        private BackgroundWorker bgwNews;
        private BackgroundWorker bgwPopular;
        private BackgroundWorker bgwHighRated;
        private BackgroundWorker bgwRecommended;

        public MainWindow()
        {
            InitializeComponent();
            CommonLibrary.MinBookToCountVote = 15;
            bgwNews = new BackgroundWorker();
            bgwPopular = new BackgroundWorker();
            bgwHighRated = new BackgroundWorker();
            bgwRecommended = new BackgroundWorker();
            LoginWindow loginWindow = new LoginWindow();

            if (!CheckConnection()) this.Close();

            InitializeBackgroundWorkers();
            loginWindow.ShowDialog();
            if (loginWindow.LoggedIn)
            {
                if (loginWindow.User.IsAdmin)
                {
                    AdminPanel.AdminPanel adminPanel = new AdminPanel.AdminPanel();
                    adminPanel.Show();
                }
                CommonLibrary.LogInUser(loginWindow.User);
                lblGreeting.Content = $"Welcome {CommonLibrary.LoggedInUser.Username},";
                bgwNews.RunWorkerAsync();
                bgwPopular.RunWorkerAsync();
                bgwHighRated.RunWorkerAsync();
                bgwRecommended.RunWorkerAsync();
            }
            else this.Close();
        }

        private void InitializeBackgroundWorkers()
        {
            bgwNews.DoWork += new DoWorkEventHandler(bgwNews_DoWork);
            bgwPopular.DoWork += new DoWorkEventHandler(bgwPopular_DoWork);
            bgwHighRated.DoWork += new DoWorkEventHandler(bgwHighRated_DoWork);
            bgwRecommended.DoWork += new DoWorkEventHandler(bgwRecommended_DoWork);
        }

        private void bgwNews_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Latest Book Additions Return
            List<Book> list = SqlHandler.GetNewsList().Content;
            this.Dispatcher.Invoke(() =>
            {
                if (!(list.Count > 0))
                {
                    MessageBox.Show("Error getting news list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (Book book in list)
                {
                    if (book != null)
                        wrapNews.Children.Add(new BookFrame(book));
                }
            });
        }

        private void bgwPopular_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Books with most distinct ratings
            List<Book> list = SqlHandler.GetPopularList().Content;
            this.Dispatcher.Invoke(() =>
            {
                if (!(list.Count > 0))
                {
                    MessageBox.Show("Error getting popular list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (Book book in list)
                {
                    if (book != null)
                        wrapPopular.Children.Add(new BookFrame(book));
                }
            });
        }

        private void bgwHighRated_DoWork(object sender, DoWorkEventArgs e)
        {
            //SQL Books with highest rating
            List<Book> list = SqlHandler.GetHighRatedList().Content;
            this.Dispatcher.Invoke(() =>
            {
                if (!(list.Count > 0))
                {
                    MessageBox.Show("Error getting high rated list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (Book book in list)
                {
                    if (book != null)
                        wrapHighRated.Children.Add(new BookFrame(book));
                }
            });
        }

        private void bgwRecommended_DoWork(object sender, DoWorkEventArgs e)
        {
            // User-Based Collaborative Filtering Algorithm
            List<Book> list = SqlHandler.GetRecommendList().Content;
            this.Dispatcher.Invoke(() =>
            {
                if (!(list.Count > 0))
                {
                    //MessageBox.Show("Error getting recommendation list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (Book book in list)
                {
                    if (book != null)
                        wrapRecommended.Children.Add(new BookFrame(book));
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
