using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Requests
{
    public class StationGetRequest : Token
    {
        public long StationCode { get; set; }
        public string Channel { get; set; } = string.Empty;
    }
}
