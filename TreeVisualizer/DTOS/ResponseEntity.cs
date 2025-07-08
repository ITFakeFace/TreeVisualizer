using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.DTOS
{
    public class ResponseEntity<T>
    {
        public bool Status { get; set; }
        public int ResponseCode { get; set; }
        public string? StatusMessage { get; set; }
        public T? Data { get; set; }
    }

}
