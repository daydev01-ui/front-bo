namespace BCP.QRBackOffice.Models.DTOs
{
    public class BusinessAddAdminDTO
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentExtension { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string CredentialsEmail { get; set; } = string.Empty;
    }
}
