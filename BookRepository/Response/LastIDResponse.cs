using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository.Response
{
    public class LastIDResponse
    {
        public string LastID { get; set; }
        public bool Success = false;
        public string ErrorText = String.Empty;
    }
}
