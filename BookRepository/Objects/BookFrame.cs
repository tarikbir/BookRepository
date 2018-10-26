using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BookRepository
{
    public partial class BookFrame : Button
    {
        public Book Book { get; private set; }
        public User User;

        public BookFrame(Book book, User user)
        {
            Book = book;
            User = user;
            Click += new System.Windows.RoutedEventHandler(OnBookClick);

            ToolTip = Book.BookTitle;
            MaxWidth = 126;

            SetImage();
        }

        public async void SetImage()
        {
            var image = await WebHandler.GetNewImageAsync(new Uri(Book.ImageURI_M));
            if (image != null && image?.Width > 1)
            {
                var bookImage = new Image
                {
                    Source = image
                };
                AddChild(bookImage);
            }
            else
            {
                AddChild(new TextBlock() { Text = "No Image Available" });
            }
        }

        public void OnBookClick(object sender, RoutedEventArgs e)
        {
            BookViewWindow bookWindow = new BookViewWindow(Book,User);
            bookWindow.ShowDialog();
        }
    }
}
