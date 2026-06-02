using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Requests
{
    public class BusinessUpdateRequest : Token
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessChannel { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
