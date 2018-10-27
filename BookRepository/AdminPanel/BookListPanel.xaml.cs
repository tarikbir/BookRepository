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
            foreach (var item in allBookResponse.Content)
            {
                lbxBook.Items.Add(item);
            }
        }

        private void btnBookAdd_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(txtISBN.Text))
            {
                MessageBox.Show("Please enter a valid ISBN Code!","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                Book book = new Book();
                {
                    book.ISBN = txtISBN.Text;
                    book.BookTitle = txtBookTitle.Text;
                    book.BookAuthor = txtBookAuthor.Text;
                    book.Publisher = txtPublisher.Text;
                    book.ImageURI_L = txtURIL.Text;
                    book.ImageURI_M = txtURIM.Text;
                    book.ImageURI_S = txtURIS.Text;
                } 
                lbxBook.Items.Add(book);
                var AddBook = SqlHandler.AddBook(book);
            }
        }
        private void btnBookRemove_Click(object sender, RoutedEventArgs e)
        {
            if(lbxBook.SelectedIndex > -1)
            {
                Book book = (Book)lbxBook.SelectedItem;
                lbxBook.Items.Remove(lbxBook.SelectedItem);
                var RemoveBook = SqlHandler.RemoveBook(book);
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
                txtURIM.Text = book.ImageURI_M;
                txtURIS.Text = book.ImageURI_S;
            }
        }
    }
}
