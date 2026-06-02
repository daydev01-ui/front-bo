namespace BCP.QRBackOffice.Models
{
    public class Response<TResult>
    {
        public TResult? Data { get; set; }
        public string State { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public bool HasError(bool validate = true)
        {
            if (State == null) { return true; }
            if (!Helper._codes.IsMatch(State)) { return true; }
            return validate && Helper.IsEmpty(Data);
        }

        public static Response<TResult> Status(Status status) => new()
        {
            State = status.Code,
            Message = status.Message
        };

        public static implicit operator Response<TResult>(Status status)
        {
            return new Response<TResult> { State = status.Code, Message = status.Message };
        }
    }
}
