﻿using System;
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
        Book book;
        
        public BookImage(Book book)
        {
            this.book = book;
            var image = new BitmapImage();

        }
    }
}
