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
    public partial class BookViewVoteWindow : Window
    {
        private Book Book;
        public int Vote;
        private int PreviousVote;

        public BookViewVoteWindow(Book book, int vote)
        {
            InitializeComponent();
            Book = book;
            PreviousVote = vote;
            InitVotes();
        }

        private void InitVotes()
        {
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
                if (PreviousVote == i) { rbVote.IsChecked = true; }
                CheckBoxesGrid.Children.Add(rbVote);
            }
            Vote = PreviousVote;
        }

        private void btnApplyVote_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
            }
        }

        private void RadioButtonSetVote(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(((RadioButton)sender).Content.ToString(), out Vote);
        }
    }
}
