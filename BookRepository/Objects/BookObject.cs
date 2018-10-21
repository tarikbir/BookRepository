﻿using System;
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
        public Image Image { get; set; }
        public BitmapImage Bitmap { get; set; }

        public BookObject(Book book)
        {
            this.Book = book;
            this.Image = new Image();
            this.Bitmap = new BitmapImage();
            this.Bitmap.BeginInit();
            this.Bitmap.UriSource = new Uri(book.ImageURI_M, UriKind.Absolute);
            this.Bitmap.DownloadCompleted += new EventHandler(AddTextOnButton);
            this.Bitmap.CacheOption = BitmapCacheOption.OnLoad;
            this.Bitmap.EndInit();
            Image.Source = Bitmap;            
            this.Click += new System.Windows.RoutedEventHandler(OnBookClick);

            this.ToolTip = Book.BookTitle;
            this.MaxWidth = 126;
        }

        private void AddTextOnButton(object sender, EventArgs e)
        {
            if (((BitmapImage)sender).Width > 1)
            {
                this.AddChild(Image);
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
    }
}
