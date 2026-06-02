namespace BCP.QRBackOffice.Models.Connectors.User.v1.Responses
{
    public class UserRegisterResponse
    {
        public string BusinessCode { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public ClientLoginResponse Client { get; set; } = new();
        public DeviceLoginResponse Device { get; set; } = new();
    }

    public class ClientLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PasswordExpirationDate { get; set; } = string.Empty;
        public long Id { get; set; }
    }

    public class DeviceLoginResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }


}
