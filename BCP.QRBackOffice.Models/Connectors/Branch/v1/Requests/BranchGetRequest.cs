using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests
{
    public class BranchGetRequest : Token
    {
        public long BranchCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
