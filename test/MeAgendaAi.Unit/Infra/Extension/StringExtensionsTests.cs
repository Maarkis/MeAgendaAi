using FluentAssertions;
using MeAgendaAi.Infra.Extension;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.Extension;

public class StringExtensionsTests
{
	[TestCase("A1B2C3D4E5F6G7H8I9J10", "12345678910")]
	[TestCase("a123b456c789", "123456789")]
	[TestCase("1", "1")]
	[TestCase("a", "")]
	[TestCase("", "")]
	[TestCase(null, "")]
	[TestCase("'\"\\'!@#$%¨&¨*()_+=-[]{}?L/||", "")]
	public void OnlyNumbers_ShouldReturnNumbersOnly(string source, string expected)
	{
		var result = source.OnlyNumbers();

		result.Should().Be(expected);
	}
}