using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Requests
{
    public class LoginParameter : Token
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }

    public class ChangePasswordParameter : Token
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewPasswordConfirmation { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
