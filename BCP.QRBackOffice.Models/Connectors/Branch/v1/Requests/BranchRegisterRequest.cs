using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests
{
    public class BranchRegisterRequest : Token
    {
        public long BusinessCode { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string CreationUser { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
