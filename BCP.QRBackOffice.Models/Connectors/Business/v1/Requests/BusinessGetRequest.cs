using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Requests
{
    public class BusinessGetRequest : Token
    {
        public long BusinessCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
