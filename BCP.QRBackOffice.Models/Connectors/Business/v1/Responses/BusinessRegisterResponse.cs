namespace BCP.QRBackOffice.Models.Connectors.Business.v1.Responses
{
    public class BusinessRegisterResponse
    {
        public long BusinessCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAbreviation { get; set; } = string.Empty;
        public string BusinessChannel { get; set; } = string.Empty;
    }
}
