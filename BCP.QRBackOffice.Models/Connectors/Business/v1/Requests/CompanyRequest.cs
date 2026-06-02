using System;
using System.Collections.Generic;
using System.Text;

namespace BCP.Models.Requests
{
    public class CompanyRequest
    {
        public string AppUserId { get; set; }
        public string PublicToken { get; set; }
        public string BusinessCode {  get; set; }
    }
}
