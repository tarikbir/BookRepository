using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository
{
    public class User : IComparable
    {
        public UInt32 UserID { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public UInt32? Age { get; set; }
        public bool IsAdmin { get; set; }

        public int CompareTo(object obj)
        {
            try
            {
                return (((User)obj).UserID < this.UserID) ? 1 : -1;
            }
            catch
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return String.Concat(Username, " (", UserID, ")");
        }
    }
}
