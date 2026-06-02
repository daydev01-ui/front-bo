namespace BCP.QRBackOffice.Models.DTOs
{
    public class BusinessDTO
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public bool IsUserAdmin { get; set; }
        public string CreationUser { get; set; } = string.Empty;
        public string CreationDate { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string ModificationDate { get; set; } = string.Empty;
        public bool State { get; set; }
    }
}
