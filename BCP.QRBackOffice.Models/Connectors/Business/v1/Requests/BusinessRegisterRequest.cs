using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Requests
{
    public class BusinessRegisterRequest : Token
    {
        public string BusinessName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
