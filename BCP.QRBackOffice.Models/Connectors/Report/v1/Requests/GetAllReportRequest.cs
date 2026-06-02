using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCP.QRBackOffice.Models.Connectors.Report.v1.Requests
{
    public class GetAllReportRequest
    {
        public string? PublicToken { get; set; }
        public string? AppUserId { get; set; }
        public string? BusinessCode { get; set; }

    }
}
