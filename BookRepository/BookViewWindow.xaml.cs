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
        private int Vote = -1;
        private int PreviousVote = -1;

        public BookViewWindow(Book book)
        {
            InitializeComponent();
            Book = book;
            InitVotes();
        }

        private void InitVotes()
        {
            var voteResponse = SqlHandler.GetVote(CommonLibrary.LoggedInUser.UserID.ToString(), Book.ISBN);
            for (int i = 0; i <= 10; i++)
            {
                RadioButton rbVote = new RadioButton()
                {
                    Name = "Vote" + i,
                    Content = i.ToString(),
                    IsTabStop = false,
                    Focusable = false,
                    FontSize = 9
                };
                rbVote.Checked += new RoutedEventHandler(RadioButtonSetVote);
                if (voteResponse.Success) if (voteResponse.Content == i) { rbVote.IsChecked = true; PreviousVote = i; }
                CheckBoxesGrid.Children.Add(rbVote);
            }
            Vote = PreviousVote;
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

        private void btnApplyVote_Click(object sender, RoutedEventArgs e)
        {
            Response.BaseResponse response;
            if (PreviousVote == Vote) return;
            if (PreviousVote == -1)
            {
                response = SqlHandler.AddVote(CommonLibrary.LoggedInUser.UserID.ToString(), Book.ISBN, Vote);
            }
            else
            {
                response = SqlHandler.UpdateVote(CommonLibrary.LoggedInUser.UserID.ToString(), Book.ISBN, Vote);
            }
            if (response.Success)
            {
                MessageBox.Show("Successfully added the vote.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Unknown error occured while adding the vote.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void RadioButtonSetVote(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(((RadioButton)sender).Content.ToString(), out Vote);
        }

        private int HashBookName (string ISBN)
        {
            int hashNumber = 0, i = 0;
            var reg = new Regex("[0-9]");
            var match = reg.Matches(ISBN);
            foreach (var item in match)
            {
                int num = 0;
                Int32.TryParse(item.ToString(), out num);
                hashNumber += num*++i;
            }
            return hashNumber%3;
        }
    }
}
