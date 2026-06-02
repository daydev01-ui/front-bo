namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses
{
    public class BranchRegisterResponse
    {
        public long BranchCode { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCity { get; set; } = string.Empty;
        public long BusinessCode { get; set; }
        public long UserId { get; set; }
    }
}
