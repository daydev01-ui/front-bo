using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Requests
{
    public class StationGetAllRequest : Token
    {
        public long BusinessCode { get; set; }
        public long BranchCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
