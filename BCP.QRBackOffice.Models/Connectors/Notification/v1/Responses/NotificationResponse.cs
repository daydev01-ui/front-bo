namespace BCP.QRBackOffice.Models.Connectors.Notification.v1.Responses
{
    public class NotificationResponse
    {
        public RespData Push { get; set; } = new();
        public RespData Email { get; set; } = new();
        public RespData Sms { get; set; } = new();
        public RespData WhatsApp { get; set; } = new();
    }

    public class RespData
    {
        public ContentData Data { get; set; } = new();
        public string State { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ContentData
    {
        public int Failure { get; set; }
        public int Success { get; set; }
        public Fail[] ClientsFailure { get; set; } = { };
    }

    public class Fail
    {
        public string Cic { get; set; } = string.Empty;
        public string Idc { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Motive { get; set; } = string.Empty;
    }
}
