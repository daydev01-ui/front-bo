using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests
{
    public class BranchGetAllRequest : Token
    {
        public long BusinessCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
