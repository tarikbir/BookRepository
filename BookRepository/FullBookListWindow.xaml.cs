using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookRepository
{
    public partial class FullBookListWindow : Window
    {
        List<Book> FullList;
        int index;
        int increment = 100;

        public FullBookListWindow()
        {
            InitializeComponent();
            index = 100;
            var response = SqlHandler.GetAllBooks();
            if (response.Success)
            {
                FullList = response.Content;
                var firstList = (from t in FullList select t).Take(index);
                UpdateList(firstList);
            }
            else
            {
                MessageBox.Show("There was an error while getting the full book list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UpdateList(IEnumerable<Book> list)
        {
            wrapBooks.Children.Clear();
            foreach (var item in list)
            {
                if (item != null)
                    wrapBooks.Children.Add(new BookFrame(item));
            }
        }

        private void AddToList(IEnumerable<Book> list)
        {
            foreach (var item in list)
            {
                if (item != null)
                    wrapBooks.Children.Add(new BookFrame(item));
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var list = from t in FullList where t.BookTitle.Contains(txtSearch.Text) || t.ISBN.Contains(txtSearch.Text) || t.BookAuthor.Contains(txtSearch.Text) select t;
            UpdateList(list);
        }

        private void mainScrollBar_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                var list = (from t in FullList select t).Skip(index).Take(index + increment);
                AddToList(list);
                index += increment;
            }
        }
    }
}
