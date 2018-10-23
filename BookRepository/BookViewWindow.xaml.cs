using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class BookViewWindow : Window
    {
        private Book Book;
        private User User;

        public BookViewWindow(Book book, User user)
        {
            InitializeComponent();
            Book = book;
            User = user;

            InitUser();
        }

        private void InitUser()
        {
            var voteResponse = SqlHandler.GetVote(User.UserID.ToString(), Book.ISBN);

            if (voteResponse.Success)
            {
                string votedBox = "Vote"+voteResponse.Content.ToString();
                foreach (var item in Grid.Children.OfType<RadioButton>())
                {
                    if (item.Name == votedBox)
                    {
                        item.IsChecked = true;
                        break;
                    }
                }
            }
        }

        private void btnReadBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string executableLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                int fileNo = HashBookName(Book.ISBN);
                string fileName = System.IO.Path.Combine(executableLocation, "Books\\book" + fileNo + ".pdf");
                System.Diagnostics.Process.Start(fileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Could not open a PDF File\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Book != null)
            {
                txtName.Text = Book.BookTitle + " (" + Book.ISBN + ")";
                txtAuthor.Text = Book.BookAuthor;
                txtPublishDate.Text = Book.YearOfPublication + " - " + Book.Publisher;
                this.Title = "Viewing " + Book.BookTitle;
                this.imgBookImage.Source = new BitmapImage(new Uri(Book.ImageURI_L, UriKind.Absolute));
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

        private int HashBookName (string ISBN)
        {
            int hashNumber = 0, i = 0;
            var reg = new Regex("[0-9]");
            var match = reg.Matches(ISBN);
            foreach (var item in match)
            {
                Int32.TryParse(item.ToString(), out int num);
                hashNumber += num*++i;
            }
            return hashNumber%3;
        }
    }
}
