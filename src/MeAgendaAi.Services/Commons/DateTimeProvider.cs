using MeAgendaAi.Domains.Interfaces.Commons;

namespace MeAgendaAi.Services.Commons;

public sealed class DateTimeProvider : IDateTimeProvider
{
	public DateTime Now => DateTime.Now;
	
	public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
	
	public DateOnly Tomorrow => Today.AddDays(1);

	public DateOnly Yesterday => Today.AddDays(-1);
	
	public DateTime EndOfDay() => 
		new(
			year: Now.Year,
			month: Now.Month,
			day: Now.Day,
			hour: 23,
			minute: 59,
			second: 59
		);
	
	public DateTime EndOfDay(DateTime date) => 
		new(
			year: date.Year,
			month: date.Month,
			day: date.Day,
			hour:23,
			minute: 59,
			second: 59
			);
	
	public DateTime EndOfDay(DateOnly date) => 
		new(
			year: date.Year,
			month: date.Month,
			day: date.Day,
			hour: 23,
			minute: 59,
			second: 59
			);
	
	public DateTime SetTime(int hour, int minute, int second) => 
		new(
			year: Now.Year,
			month: Now.Month,
			day: Now.Day,
			hour: hour,
			minute: minute,
			second: second
			);
	
	public DateTime SetTime(int hour, int minute) =>
		SetTime(hour, minute, second: 0);
	
	public DateTime SetTime(int hour) =>
		SetTime(hour: hour, minute: 0);

	public DateTime SetTime(DateTime date, int hour, int minute, int second) =>
		new(
			year: date.Year,
			month: date.Month,
			day: date.Day,
			hour: hour,
			minute: minute,
			second: second
		);

	public DateOnly GoToLastDayOfMonth() => new(year: Now.Year, month: Now.Month, LastDayMonth());
	
	public DateOnly GoToLastDayOfMonth(DateTime date) => new(year: date.Year, month: date.Month, LastDayMonth(date));
	
	public DateOnly GoToLastDayOfMonth(DateOnly date) => new(year: date.Year, month: date.Month, LastDayMonth(date));
	
	public int LastDayMonth() => LastDayOfMonth(Now.Month, Now.Year);
	
	public int LastDayMonth(DateTime date) => LastDayOfMonth(date.Month, date.Year);
	
	public int LastDayMonth(DateOnly date) => LastDayOfMonth(date.Month, date.Year);
	
	public DateOnly OneYearAgo() => Today.AddYears(-1);
	
	public DateOnly OneYearAgo(DateOnly date) => date.AddYears(-1);
	
	public DateOnly OneYearAgo(DateTime date) => DateOnly.FromDateTime(date.AddYears(-1));
	
	public DateOnly OneYearForward() => Today.AddYears(1);
	
	public DateOnly OneYearForward(DateOnly date) => date.AddYears(1);
	
	public DateOnly OneYearForward(DateTime date) => DateOnly.FromDateTime(date.AddYears(1));
	
	public TimeOnly CurrentTime() => TimeOnly.FromDateTime(Now);


	public bool IsWeekend(DateTime date) => Weekend(date);

	public bool IsWeekend(DateOnly date) => Weekend(date.ToDateTime(new TimeOnly()));
	
	private static bool Weekend(DateTime date) => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
	private static int LastDayOfMonth(int month, int year) => DateTime.DaysInMonth(month: month, year: year);
}