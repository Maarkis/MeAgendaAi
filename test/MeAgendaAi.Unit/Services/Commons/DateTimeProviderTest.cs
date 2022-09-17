using System;
using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Services.Commons;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.Commons;

public class DateTimeProviderTest
{
	private readonly DateTimeProvider _dateTimeProvider;
	private readonly Faker _faker;

	public DateTimeProviderTest()
	{
		_dateTimeProvider = new DateTimeProvider();
		_faker = new Faker("pt_BR");
	}

	[Test]
	public void Now_ShouldReturnCurrentDateAndTimeCorrectly()
	{
		_dateTimeProvider.Now.Should().BeCloseTo(DateTime.Now, 0.1.Seconds());
	}

	[Test]
	public void Today_ShouldReturnCurrentDateCorrectly()
	{
		var today = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

		_dateTimeProvider.Today.Should().Be(today);
	}

	[Test]
	public void Tomorrow_ShouldReturnTomorrowDateCorrectly()
	{
		var tomorrow = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(1);

		_dateTimeProvider.Tomorrow.Should().Be(tomorrow);
	}

	[Test]
	public void Yesterday_ShouldReturnYesterdayDateCorrectly()
	{
		var yesterday = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddDays(-1);

		_dateTimeProvider.Yesterday.Should().Be(yesterday);
	}

	[Test]
	public void EndOfDayNow_ShouldReturnTheCurrentDateAtTheEndOfTheDayCorrectly()
	{
		var endOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);

		_dateTimeProvider.EndOfDay().Should().Be(endOfDay);
	}

	[Test]
	public void EndOfDayDateTime_ShouldReturnTheEndOfDayOfASpecifiedDateCorrectly()
	{
		var endOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);

		_dateTimeProvider.EndOfDay(DateTime.Now).Should().Be(endOfDay);
	}

	[Test]
	public void EndOfDayDateOnly_ShouldReturnTheEndOfDayOfASpecifiedDateCorrectly()
	{
		var currentDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
		var endOfDay = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);

		_dateTimeProvider.EndOfDay(currentDate).Should().Be(endOfDay);
	}

	[Test]
	public void SetTimeHourAndMinuteAndSecond_ShouldReturnDateTimeWithSpecifiedHourAndMinuteAndSecondCorrectly()
	{
		var hour = _faker.Random.Int(min: 1, max: 23);
		var minute = _faker.Random.Int(min: 1, max: 59);
		var second = _faker.Random.Int(min: 1, max: 59);
		var expectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute,
			second);

		var result = _dateTimeProvider.SetTime(hour, minute, second);

		result.Should().Be(expectedDate);
	}

	[Test]
	public void SetTimeHourAndMinute_ShouldReturnDateTimeWithSpecifiedHourAndMinuteAndSecondCorrectly()
	{
		var hour = _faker.Random.Int(min: 1, max: 23);
		var minute = _faker.Random.Int(min: 1, 59);
		var expectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0);

		var result = _dateTimeProvider.SetTime(hour, minute);

		result.Should().Be(expectedDate);
	}

	[Test]
	public void SetTimeHour_ShouldReturnDateTimeWithSpecifiedHourAndMinuteAndSecondCorrectly()
	{
		var hour = _faker.Random.Int(min: 1, max: 23);
		var expectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, 0, 0);

		var result = _dateTimeProvider.SetTime(hour);

		result.Should().Be(expectedDate);
	}

	[Test]
	public void SetTimeSpecifiedDate_ShouldReturnDateTimeWithSpecifiedHourAndMinuteAndSecondCorrectly()
	{
		var hour = _faker.Random.Int(min: 1, max: 23);
		var minute = _faker.Random.Int(min: 1, max: 59);
		var second = _faker.Random.Int(min: 1, max: 59);
		var randomDate = _faker.Date.Past();
		var expectedDate = new DateTime(randomDate.Year, randomDate.Month, randomDate.Day, hour, minute, second);

		var result = _dateTimeProvider.SetTime(hour, minute, second, randomDate);

		result.Should().Be(expectedDate);
	}

	[Test]
	public void GoToLastDayOfMonth_ShouldReturnTheLastDateOfTheCurrentMonthCorrectly()
	{
		var expectedDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month,
			DateTime.DaysInMonth(year: DateTime.Today.Year, month: DateTime.Today.Month));

		_dateTimeProvider.GoToLastDayOfMonth().Should().Be(expectedDate);
	}

	[Test]
	public void GoToLastDayOfMonthDateTime_ShouldReturnTheLastDateOfTheMonthForASpecificDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var expectedDate = new DateOnly(randomDate.Year, randomDate.Month,
			DateTime.DaysInMonth(year: randomDate.Year, month: randomDate.Month));

		_dateTimeProvider.GoToLastDayOfMonth(randomDate).Should().Be(expectedDate);
	}

	[Test]
	public void GoToLastDayOfMonthDateOnly_ShouldReturnTheLastDateOfTheMonthForASpecificDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var randomDateInDateOnly = new DateOnly(randomDate.Year, randomDate.Month, randomDate.Day);
		var expectedDate = new DateOnly(randomDate.Year, randomDate.Month,
			DateTime.DaysInMonth(year: randomDate.Year, month: randomDate.Month));

		_dateTimeProvider.GoToLastDayOfMonth(randomDateInDateOnly).Should().Be(expectedDate);
	}

	[Test]
	public void LastDayMonth_ShouldReturnTheLastDayOfTheCurrentMonthCorrectly()
	{
		var expectedDay = DateTime.DaysInMonth(year: DateTime.Now.Year, month: DateTime.Now.Month);

		_dateTimeProvider.LastDayMonth().Should().Be(expectedDay);
	}

	[Test]
	public void LastDayMonthDateTime_ShouldReturnTheLastDateOfTheMonthForASpecificDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var expectedDay = DateTime.DaysInMonth(year: randomDate.Year, month: randomDate.Month);

		_dateTimeProvider.LastDayMonth(randomDate).Should().Be(expectedDay);
	}

	[Test]
	public void LastDayMonthDateOnly_ShouldReturnTheLastDateOfTheMonthForASpecificDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var randomDateInDateOnly = new DateOnly(randomDate.Year, randomDate.Month, randomDate.Day);
		var expectedDay = DateTime.DaysInMonth(year: randomDate.Year, month: randomDate.Month);

		_dateTimeProvider.LastDayMonth(randomDateInDateOnly).Should().Be(expectedDay);
	}

	[Test]
	public void OneYearAgo_ShouldReturnOneYearBehindTheCurrentDayCorrectly()
	{
		var expectedDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddYears(-1);

		_dateTimeProvider.OneYearAgo().Should().Be(expectedDate);
	}

	[Test]
	public void OneYearAgoDateTime_ShouldReturnOneYearBehindASpecifiedDateCorrectly()
	{
		var expectedDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddYears(-1);

		_dateTimeProvider.OneYearAgo(DateTime.Today).Should().Be(expectedDate);
	}

	[Test]
	public void OneYearAgoDateOnly_ShouldReturnOneYearBehindASpecifiedDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var randomDateInDateOnly = new DateOnly(randomDate.Year, randomDate.Month, randomDate.Day);
		var expectedDate = randomDateInDateOnly.AddYears(-1);

		_dateTimeProvider.OneYearAgo(randomDateInDateOnly).Should().Be(expectedDate);
	}

	[Test]
	public void OneYearForward_ShouldReturnOneYearAheadTheCurrentDayCorrectly()
	{
		var expectedDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddYears(1);

		_dateTimeProvider.OneYearForward().Should().Be(expectedDate);
	}

	[Test]
	public void OneYearAgoDateTime_ShouldReturnOneYearAheadASpecifiedDateCorrectly()
	{
		var expectedDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddYears(1);

		_dateTimeProvider.OneYearForward(DateTime.Today).Should().Be(expectedDate);
	}

	[Test]
	public void OneYearAgoDateOnly_ShouldReturnOneYearAheadASpecifiedDateCorrectly()
	{
		var randomDate = _faker.Date.Past();
		var randomDateInDateOnly = new DateOnly(randomDate.Year, randomDate.Month, randomDate.Day);
		var expectedDate = randomDateInDateOnly.AddYears(1);

		_dateTimeProvider.OneYearForward(randomDateInDateOnly).Should().Be(expectedDate);
	}

	[Test] // TODO: review test implementation :)
	public void CurrentTime_ShouldReturnTheCurrentTimeCorrectly()
	{
		var currentTimeExpected = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
			DateTime.Now.Millisecond);

		_dateTimeProvider.CurrentTime().ToTimeSpan().Should()
			.BeCloseTo(currentTimeExpected.ToTimeSpan(), 0.1.Seconds());
	}

	[Test]
	public void IsWeekendDateTime_ShouldReturnTrueWhenDateIsOnTheWeekend()
	{
		var dateInWeekend = RandomDateInWeekend();

		_dateTimeProvider.IsWeekend(dateInWeekend).Should().BeTrue();
	}

	[Test]
	public void IsWeekendDateOnly_ShouldReturnTrueWhenDateIsOnTheWeekend()
	{
		var dateInWeekend = DateOnly.FromDateTime(RandomDateInWeekend());

		_dateTimeProvider.IsWeekend(dateInWeekend).Should().BeTrue();
	}

	private DateTime RandomDateInWeekend()
	{
		DateTime date;
		do date = _faker.Date.Past();
		while (NotWeekend(date));

		return date;
	}

	private static bool NotWeekend(DateTime date) => date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
}