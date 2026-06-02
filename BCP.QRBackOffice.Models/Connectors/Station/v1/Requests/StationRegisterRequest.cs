using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Requests
{
    public class StationRegisterRequest : Token
    {
        public string AtmName { get; set; } = string.Empty;
        public long BusinessCode { get; set; }
        public long BranchCode { get; set; }
        public long UserId { get; set; }
        public string CellPhone { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
    }
}
