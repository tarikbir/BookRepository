using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRepository.Response
{
    public class GenericResponse<T> : BaseResponse
    {
        public T Content { get; set; }

        public GenericResponse()
        {}
    }
}
