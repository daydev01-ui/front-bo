namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Responses
{
    public class BusinessUpdateResponse
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string BusinessChannel { get; set; } = string.Empty;
    }
}
