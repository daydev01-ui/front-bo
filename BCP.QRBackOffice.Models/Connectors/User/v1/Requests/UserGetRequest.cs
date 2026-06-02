using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.User.v1.Requests
{
    public class UserGetRequest : Token
    {
        public long UserCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
