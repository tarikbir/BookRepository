using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository.Response
{
    public class BaseResponse
    {
        public bool Success = false;
        public string ErrorText = String.Empty;
    }
}
