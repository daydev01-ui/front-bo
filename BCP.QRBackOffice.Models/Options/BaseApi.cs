namespace BCP.QRBackOffice.Models.Options
{
    public class Token
    {
        public string PublicToken { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
    }

    public class BaseApiOptions
    {
        public string Url { get; set; } = string.Empty;
        public TimeSpan Timeout { get; set; }
    }

    public class BaseIAMOptions : BaseApiOptions
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PublicToken { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
    }


    public class Channel : BaseIAMOptions
    {
        public string ChannelName { get; set; } = string.Empty;
        public string SegCryptName { get; set; } = string.Empty;
    }

    public class NotificationOptions : BaseApiOptions
    {
        public string Application { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;
        public string EmailName { get; set; } = string.Empty;
        public string PushTitle { get; set; } = string.Empty;
        public string PushMessage { get; set; } = string.Empty;
    }
}
