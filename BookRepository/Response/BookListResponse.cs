using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository.Response
{
    public class BookListResponse : BaseResponse
    {
        public List<Book> Books { get; set; }

        public BookListResponse()
        {
            Books = new List<Book>();
        }
    }
}
