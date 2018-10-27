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
    public partial class BookListPanel : Window
    {
        public BookListPanel()
        {
            InitializeComponent();
        }

        private void btnBookList_Click(object sender, RoutedEventArgs e)
        {
            var allBookResponse = SqlHandler.GetAllBooks();
            allBookResponse.Content.Sort();
            lbxBook.Items.Clear();

            Dispatcher.Invoke(() =>
            {
                foreach (var item in allBookResponse.Content)
                {
                    lbxBook.Items.Add(item);
                }
            });
        }

        private void btnBookAdd_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(txtISBN.Text) || String.IsNullOrWhiteSpace(txtBookTitle.Text) || String.IsNullOrWhiteSpace(txtBookAuthor.Text)
                || String.IsNullOrWhiteSpace(txtPublisher.Text) || String.IsNullOrWhiteSpace(txtYearOfPublication.Text) || String.IsNullOrWhiteSpace(txtURIL.Text)
                || String.IsNullOrWhiteSpace(txtURIM.Text) || String.IsNullOrWhiteSpace(txtURIS.Text))
            {
                MessageBox.Show("Information cannot be empty!","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                Book book = new Book()
                {
                    ISBN = txtISBN.Text,
                    BookTitle = txtBookTitle.Text,
                    BookAuthor = txtBookAuthor.Text,
                    Publisher = txtPublisher.Text,
                    ImageURI_L = txtURIL.Text,
                    ImageURI_M = txtURIM.Text,
                    ImageURI_S = txtURIS.Text
                };
                var addBookResponse = SqlHandler.AddBook(book);
                if (addBookResponse.Success)
                {
                    MessageBox.Show("Successfully added " + book.BookTitle + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    lbxBook.Items.Add(book);
                }
                else
                    MessageBox.Show("Error adding " + book.BookTitle + ".\n\n" + addBookResponse.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void btnBookRemove_Click(object sender, RoutedEventArgs e)
        {
            if(lbxBook.SelectedIndex > -1)
            {
                Book book = (Book)lbxBook.SelectedItem;
                var removeBookResponse = SqlHandler.RemoveBook(book);
                if (removeBookResponse.Success)
                {
                    MessageBox.Show("Successfully removed " + book.BookTitle + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    lbxBook.Items.RemoveAt(lbxBook.SelectedIndex);
                }
                else
                    MessageBox.Show("Error removing " + book.BookTitle + ".\n\n" + removeBookResponse.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbxBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbxBook.SelectedIndex > -1)
            {
                Book book = (Book)lbxBook.SelectedItem;
                txtISBN.Text = book.ISBN;
                txtBookTitle.Text = book.BookTitle;
                txtBookAuthor.Text = book.BookAuthor;
                txtPublisher.Text = book.Publisher;
                txtYearOfPublication.Text = book.YearOfPublication.ToString();
                txtURIL.Text = book.ImageURI_L;
                txtURIM.Text = book.ImageURI_M;
                txtURIS.Text = book.ImageURI_S;
            }
        }
    }
}
