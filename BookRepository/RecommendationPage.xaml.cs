using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BookRepository
{
    public partial class RecommendationPage : Window
    {
        List<Book> bookList;
        private BackgroundWorker bgwRecommender;

        public RecommendationPage()
        {
            InitializeComponent();
            bgwRecommender = new BackgroundWorker();
            bgwRecommender.DoWork += new DoWorkEventHandler(bgwRecommender_DoWork);
            bgwRecommender.ProgressChanged += new ProgressChangedEventHandler(bgwRecommender_ProgressChanged);
            bgwRecommender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwRecommender_RunWorkerCompleted);
            bgwRecommender.WorkerReportsProgress = true;
            bgwRecommender.RunWorkerAsync();
        }

        private void bgwRecommender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.lbInfo.Content = "Loading complete. You can see the books recommended to you.";
            this.pbBgw.Visibility = Visibility.Hidden;
        }

        private void bgwRecommender_DoWork(object sender, DoWorkEventArgs e)
        {
            int bookTotal = 0;
            var books = Recommender.Recommend(CommonLibrary.LoggedInUser);
            bookTotal = books.Count();

            bookList = new List<Book>();
            foreach (var item in books)
            {
                var bookResponse = SqlHandler.GetBook(item);
                if (bookResponse.Success)
                    bookList.Add(bookResponse.Content);
            }

            int progress = 1;

            foreach (var item in bookList)
            {
                bgwRecommender.ReportProgress(80 * progress++ / bookTotal, item);
            }

            progress = 1;

            if (bookList.Count < 10)
            {
                var popularListResponse = SqlHandler.GetPopularList();
                if (popularListResponse.Success)
                    foreach (var item in popularListResponse.Content)
                    {
                        bgwRecommender.ReportProgress(80 + (20 * progress++ / popularListResponse.Content.Count), item);
                    }
            }
        }

        private void bgwRecommender_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AddBookToWrap((e.UserState as Book));
            pbBgw.Value = e.ProgressPercentage;
        }

        private void AddBookToWrap(Book item)
        {
            if (item != null) wrapRecommendedBooks.Children.Add(new BookFrame(item));
        }
    }
}
