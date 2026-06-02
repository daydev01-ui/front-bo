namespace BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses
{
    public class BranchResponse
    {
        public long BranchCode { get; set; }
        public string BranchCity { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public long BusinessCode { get; set; }
        public long UserCode { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
        public DateTime ModificationDate { get; set; }
        public bool State { get; set; }
    }
}
