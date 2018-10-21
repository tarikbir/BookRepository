using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BookRepository
{
    public partial class BookObject : Button
    {
        public Book Book { get; set; }
        public Image image = new Image();

        public BookObject(Book book)
        {
            this.Book = book;
            
            this.Click += new System.Windows.RoutedEventHandler(OnBookClick);
            this.Loaded += new RoutedEventHandler(GetImageResult);

            this.ToolTip = Book.BookTitle;
            this.MaxWidth = 126;
        }

        private void AddTextOnButton(object sender, EventArgs e)
        {
            if (((BitmapImage)sender).Width > 1)
            {
                this.AddChild(image);
            }
            else
            {
                this.AddChild(new TextBlock() { Text = "No Image Available" });
            }
        }

        public void OnBookClick(object sender, RoutedEventArgs e)
        {
            BookViewWindow bookWindow = new BookViewWindow(Book);
            bookWindow.ShowDialog();
        }

        public void GetImageResult(object sender, RoutedEventArgs e)
        {
            BitmapImage Bitmap = new BitmapImage();
            Bitmap.BeginInit();
            Bitmap.UriSource = new Uri(Book.ImageURI_M, UriKind.Absolute);
            Bitmap.DownloadCompleted += new EventHandler(AddTextOnButton);
            Bitmap.CacheOption = BitmapCacheOption.OnLoad;
            Bitmap.EndInit();
            image.Source = Bitmap;
            this.UpdateLayout();
        }
    }
}
