using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public class User
    {
        public UInt32 UserID { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public UInt32? Age { get; set; }
        public bool IsAdmin { get; set; }
    }
}
