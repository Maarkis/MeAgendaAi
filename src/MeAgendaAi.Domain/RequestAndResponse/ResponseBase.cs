namespace MeAgendaAi.Domains.RequestAndResponse
{
    public class BaseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public BaseMessage(string message = "", bool success = false) => (Message, Success) = (message, success);
    }

    public class SuccessMessage<T> : BaseMessage
    {
        public T Result { get; set; }

        public SuccessMessage(T result, string message = "") : base(message, true) => Result = result;
    }

    public class ErrorMessage<T> : BaseMessage
    {
        public T Error { get; set; }
        public ErrorMessage(T error, string message = "") : base(message, false) => (Error) = (error);
    }
}
