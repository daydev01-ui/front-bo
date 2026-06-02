namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Responses
{
    public class BusinessResponse
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public bool IsUserAdmin { get; set; }
        public DateTime CreationDate { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
        public DateTime ModificationDate { get; set; }
        public bool State { get; set; }
        public string BusinessChannel { get; set; } = string.Empty;
    }
}
