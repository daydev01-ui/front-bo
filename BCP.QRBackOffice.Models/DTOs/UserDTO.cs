namespace BCP.QRBackOffice.Models.DTOs
{
    public class UserDTO
    {
        public long UserCode { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Access { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public string CreationDate { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string ModificationDate { get; set; } = string.Empty;
        public bool State {  get; set; }
    }
}