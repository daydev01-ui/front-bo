using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCP.QRBackOffice.Models.Connectors.Report.v1.Responses
{
    public class ExcelReportResponse
    {
        public byte[]? Bytes { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
