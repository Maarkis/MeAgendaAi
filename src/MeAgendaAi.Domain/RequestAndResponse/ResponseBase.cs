namespace MeAgendaAi.Domains.RequestAndResponse
{
    /// <summary>
    /// Basic system response.
    /// </summary>
    public class BaseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Build basic system response.
        /// </summary>
        /// <param name="message"><c>Message</c>. Default message is "".</param>
        /// <param name="success">
        /// <c>True</c> if success. <c>False</c> if failure.
        /// </param>
        public BaseMessage(string message = "", bool success = false) => (Message, Success) = (message, success);
    }

    /// <summary>
    /// Basic success response with type <typeparamref name="T"/> result.
    /// </summary>
    /// <typeparam name="T">The type stored in <c>Result</c>.</typeparam>
    public class SuccessMessage<T> : BaseMessage
    {
        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Build  Success message.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="message"><c>Success message</c>. Default message is "".</param>
        public SuccessMessage(T result, string message = "") : base(message, true) => Result = result;
    }

    /// <summary>
    /// Basic error response with type <typeparamref name="T"/> result
    /// </summary>
    /// <typeparam name="T">The type stored in <c>Error</c>.</typeparam>
    public class ErrorMessage<T> : BaseMessage
    {
        /// <summary>
        /// Error
        /// </summary>
        public T Error { get; set; }

        /// <summary>
        /// Build error message.
        /// </summary>
        /// <param name="error">Error.</param>
        /// <param name="message"><c>Error message</c>. Default message is "".</param>
        public ErrorMessage(T error, string message = "") : base(message, false) => (Error) = (error);
    }
}