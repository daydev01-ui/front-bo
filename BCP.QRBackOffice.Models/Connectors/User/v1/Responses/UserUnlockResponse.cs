namespace BCP.QRBackOffice.Models.Connectors.User.v1.Responses
{
    public class UserUnlockResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
