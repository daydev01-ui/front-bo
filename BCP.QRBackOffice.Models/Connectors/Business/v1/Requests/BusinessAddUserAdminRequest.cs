using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Requests
{
    public class BusinessAddUserAdminRequest : Token
    {
        public long BusinessCode { get; set; }
        public string UpdateUser { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public long UserId { get; set; }
    }
}
