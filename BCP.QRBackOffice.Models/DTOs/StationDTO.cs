namespace BCP.QRBackOffice.Models.DTOs
{
    public class StationDTO
    {
        public long StationCode { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
        public long UserCode { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CreationUser { get; set; } = string.Empty;
        public string CreationDate { get; set; } = string.Empty;
        public string ModificationUser { get; set; } = string.Empty;
        public string ModificationDate { get; set; } = string.Empty;
        public bool State { get; set; }
    }
}
