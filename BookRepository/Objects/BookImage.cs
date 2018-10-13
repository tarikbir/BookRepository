using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BookRepository
{
    class BookImage : Image
    {
        Book Book;
        
        public BookImage(Book book)
        {
            this.Book = book;
            var image = new BitmapImage(new Uri(book.ImageURI_M,UriKind.Absolute));
            this.Source = image;
        }
    }
}
