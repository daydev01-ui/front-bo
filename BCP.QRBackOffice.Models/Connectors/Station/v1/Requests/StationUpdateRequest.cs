using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Requests
{
    public class StationUpdateRequest : Token
    {
        public long StationCode { get; set; }
        public string StationName { get; set; } = string.Empty;
        public long BranchCode { get; set; }
        public long UserCode { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
