using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public class Vote<T>
    {
        public T Content { get; set; }
        public int Rating { get; set; }
    }
}
