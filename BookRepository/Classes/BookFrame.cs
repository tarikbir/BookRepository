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

        public BookFrame()
        {
            Width = 100;
            Height = 160;
            MaxWidth = 100;
            MaxHeight = 160;
        }

        public BookFrame(Book book):this()
        {
            Book = book;
            ToolTip = Book.BookTitle;
            Click += new System.Windows.RoutedEventHandler(OnBookClick);

            SetImage();
        }

        public BookFrame(Book book, System.Windows.RoutedEventHandler action):this()
        {
            Book = book;
            ToolTip = Book.BookTitle;
            Click += action;

            SetImage();
        }

        public async void SetImage()
        {
            try
            {
                var image = await WebHandler.GetNewImageAsync(new Uri(Book.ImageURI_M));
                if (image != null && image?.Width > 1)
                {
                    var bookImage = new Image
                    {
                        Source = image,
                        Stretch = System.Windows.Media.Stretch.Fill,
                        RenderSize = new Size(98, 158),
                        Width = 98,
                        Height = 158,
                        MaxWidth = 98,
                        MaxHeight = 158
                    };
                    AddChild(bookImage);
                }
                else
                {
                    AddChild(new TextBlock() { Text = "No Image Available", TextWrapping=TextWrapping.Wrap });
                }
            }
            catch
            {
                AddChild(new TextBlock() { Text = "No Image Available", TextWrapping = TextWrapping.Wrap });
            }
        }

        public void OnBookClick(object sender, RoutedEventArgs e)
        {
            BookViewWindow bookWindow = new BookViewWindow(Book);
            bookWindow.ShowDialog();
        }
    }
}
