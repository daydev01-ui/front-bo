namespace BCP.QRBackOffice.Models.DTOs
{
    public class BranchRegisterDTO
    {
        public string BranchName { get; set; } = string.Empty;
        public string BranchCity { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentExtension { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string CredentialsEmail { get; set; } = string.Empty;
        public string NotificationsEmail { get; set; } = string.Empty;
    }
}
