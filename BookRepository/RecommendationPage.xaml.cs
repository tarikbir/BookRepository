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

namespace BookRepository
{
    public partial class RecommendationPage : Window
    {
        List<Book> bookList;

        public RecommendationPage()
        {
            InitializeComponent();
        }

        public RecommendationPage(List<string> books)
        {
            bookList = new List<Book>();
            foreach (var item in books)
            {
                var bookResponse = SqlHandler.GetBook(item);
                if (bookResponse.Success)
                    bookList.Add(bookResponse.Content);
            }

            foreach (var item in bookList)
            {
                AddBookToWrap(item);
            }

            if (bookList.Count < 10)
            {
                var popularListResponse = SqlHandler.GetPopularList();
                if (popularListResponse.Success)
                    foreach (var item in popularListResponse.Content)
                    {
                        AddBookToWrap(item);
                    }
            }
        }

        private void AddBookToWrap(Book item)
        {
            if (item != null) wrapRecommendedBooks.Children.Add(new BookFrame(item));
        }

        private void RecommendationWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
