namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Responses
{
    public class StationResponse
    {
        public long StationCode { get; set; }
        public string StationName { get; set; } = string.Empty;
        public long BusinessCode { get; set; }
        public long BranchCode { get; set; }
        public string CreationUser { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
        public DateTime ModificationDate { get; set; }
        public bool State { get; set; }
        public string Cellphone { get; set; } = string.Empty;
        public long QrgeCode { get; set; }
        public long UserCode { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
