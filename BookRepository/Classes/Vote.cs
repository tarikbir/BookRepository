using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public class Vote
    {
        public User User { get; set; }
        public Book Book { get; set; }
        public int Rating { get; set; }

        public Vote()
        {
            User = new User();
            Book = new Book();
        }
    }
}
