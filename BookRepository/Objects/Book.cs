using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BookRepository
{
    public class Book
    {
        public string ISBN { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public int YearOfPublication { get; set; }
        public string Publisher { get; set; }
        public string ImageURI_S { get; set; } //57x75 avg
        public string ImageURI_M { get; set; } //122x160 avg
        public string ImageURI_L { get; set; } //362x475 avg
    }
}