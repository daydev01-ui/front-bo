namespace BCP.QRBackOffice.Models.Connectors.Station.v1.Responses
{
    public class StationRegisterResponse
    {
        public long AtmCode { get; set; }
        public string AtmName { get; set; } = string.Empty;
        public long BusinessCode { get; set; }
        public long BranchCode { get; set; }
        public long UserId { get; set; }
        public string Cellphone { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
    }
}
