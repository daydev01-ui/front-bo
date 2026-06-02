using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.User.v1.Requests
{
    public class UserUnlockRequest : Token
    {
        public string ChannelHour { get; set; } = string.Empty;
        public string ChannelDate { get; set; } = string.Empty;
        public string BusinessCode { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
