namespace MeAgendaAi.Domains.RequestAndResponse;

public class BaseMessage
{
	public BaseMessage(string message = "", bool success = false)
	{
		(Message, Success) = (message, success);
	}

	public bool Success { get; set; }
	public string Message { get; set; }
}

public class SuccessMessage<T> : BaseMessage
{
	public SuccessMessage(T result, string message = "") : base(message, true)
	{
		Result = result;
	}

	public T Result { get; set; }
}

public class ErrorMessage<T> : BaseMessage
{
	public ErrorMessage(T error, string message = "") : base(message)
	{
		Error = error;
	}

	public T Error { get; set; }
}