namespace MeAgendaAi.Domains.RequestAndResponse
{
    public class ResponseBase<T> : ResponseBase
    {
        public T Result { get; set; }

        public ResponseBase(T result, string message = "", bool success = false) : base(message, success) => Result = result;
    }

    public class ResponseBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResponseBase(string message = "", bool success = false) => (Message, Success) = (message, success);
    }
}
