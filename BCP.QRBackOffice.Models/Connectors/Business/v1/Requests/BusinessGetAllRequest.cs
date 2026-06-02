using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Requests
{
    public class BusinessGetAllRequest : Token
    {
        public string BusinessChannel { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
