namespace BCP.QRBackOffice.Models.Connectors.User.v1.Responses
{
    public class UserResponse
    {
        public long UserCode { get; set; }
        public long BusinessCode { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Access { get; set; } = string.Empty;
        public string TypeUser { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string ModificationUser { get; set; } = string.Empty;
        public DateTime ModificationDate { get; set; }
        public bool State { get; set; }
    }
}
