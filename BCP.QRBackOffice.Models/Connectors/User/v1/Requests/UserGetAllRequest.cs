using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.User.v1.Requests
{
    public class UserGetAllRequest : Token
    {
        public long BusinessCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
