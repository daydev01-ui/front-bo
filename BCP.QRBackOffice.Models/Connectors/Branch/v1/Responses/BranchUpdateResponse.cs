namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses
{
    public class BranchUpdateResponse
    {
        public long BranchCode { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCity { get; set; } = string.Empty;
        public long UserCode { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
    }
}
