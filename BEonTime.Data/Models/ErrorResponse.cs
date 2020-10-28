using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Data.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string ErrorMessage { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
