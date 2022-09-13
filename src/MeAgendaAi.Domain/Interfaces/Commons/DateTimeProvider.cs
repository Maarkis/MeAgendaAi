namespace MeAgendaAi.Domains.Interfaces.Commons;

public interface IDateTimeProvider
{
	public DateTime Now { get; }
	public DateOnly Today { get; }
	public DateOnly Tomorrow { get; }
	public DateOnly Yesterday { get; }
	public DateTime EndOfDay();
	public DateTime EndOfDay(DateTime date);
	public DateTime EndOfDay(DateOnly date);
	public DateTime SetTime(int hour, int minute, int second);
	public DateTime SetTime(int hour, int minute);
	public DateTime SetTime(int hour);
	public DateTime SetTime(DateTime date, int hour, int minute, int second);
	public TimeOnly CurrentTime();
	public int LastDayMonth();
	public int LastDayMonth(DateTime date);
	public int LastDayMonth(DateOnly date);
	public DateOnly GoToLastDayOfMonth();
	public DateOnly GoToLastDayOfMonth(DateTime date);
	public DateOnly GoToLastDayOfMonth(DateOnly date);
	public DateOnly OneYearAgo();
	public DateOnly OneYearAgo(DateOnly date);
	public DateOnly OneYearAgo(DateTime date);
	public DateOnly OneYearForward();
	public DateOnly OneYearForward(DateOnly date);
	public DateOnly OneYearForward(DateTime date);
	public bool IsWeekend(DateTime date);
	public bool IsWeekend(DateOnly date);

}