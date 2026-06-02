using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCP.QRBackOffice.Models.Connectors.Report.v1.Requests
{
    public class ExcelReportRequest
    {
        [Required]
        public string PublicToken { get; set; } = String.Empty;
        [Required]
        public string AppUserId { get; set; } = String.Empty;
        [Required]
        public string ServiceCode { get; set; } = String.Empty;
        [Required]
        public string BusinessCode { get; set; } = String.Empty;
        [Required]
        public string StartDate { get; set; } = String.Empty;
        [Required]
        public string FinalDate { get; set; } = String.Empty;
        [Required]
        public string Currency { get; set; } = String.Empty;
        [Required]
        public string StatusPayment { get; set; } = String.Empty;
    }

}
