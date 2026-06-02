namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Responses
{
    public class StationUpdateResponse
    {
        public long StationCode { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public long BranchCode { get; set; }
        public long UserCode { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
    }
}
