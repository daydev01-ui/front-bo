using BCP.QRBackOffice.Models.Options;

namespace BCP.QRBackOffice.Models.Connectors.User.v1.Requests
{
    public class UserRegisterRequest : Token
    {
        public string BusinessCode { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string TypeUser { get; set; } = string.Empty;
        public RegisterClient Client { get; set; }
        public Device Device { get; set; }
    }

    public class RegisterClient
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentExtension { get; set; } = string.Empty;
        public string DocumentComplement { get; set; } = string.Empty;
    }

    public class Device
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
    }
}
