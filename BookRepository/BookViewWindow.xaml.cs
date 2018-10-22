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
using Microsoft.Win32;

namespace BookRepository
{
    public partial class BookViewWindow : Window
    {
        public Book book;

        public BookViewWindow()
        {
            InitializeComponent();
        }

        public BookViewWindow(Book book)
        {
            InitializeComponent();
            this.book = book;
        }

        private void btnReadBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name_of_file = "poc.pdf";
                System.Diagnostics.Process.Start(name_of_file);
            }
            catch(Exception error)
            {
                MessageBox.Show("Could not open a PDF File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (book != null)
            {
                txtName.Text = book.BookTitle + " (" + book.ISBN + ")";
                txtAuthor.Text = book.BookAuthor;
                txtPublishDate.Text = book.YearOfPublication + " - " + book.Publisher;
                this.Title = "Viewing " + book.BookTitle;
                this.imgBookImage.Source = new BitmapImage(new Uri(book.ImageURI_L, UriKind.Absolute));
            }
            else
            {
                txtName.Text = " ";
                txtAuthor.Text = " ";
                txtPublishDate.Text = " ";
                this.Title = "Could Not Find Book";
                btnReadBook.IsEnabled = false;
            }
        }
    }
}
