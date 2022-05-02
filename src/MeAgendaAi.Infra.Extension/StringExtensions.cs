using static System.String;

namespace MeAgendaAi.Infra.Extension;

public static class StringExtensions
{
	private const int StartIndex = 0;

	public static string OnlyNumbers(this string source)
	{
		if (IsNullOrEmpty(source))
			return Empty;

		var onlyNumbers = new char[source.Length];
		var lastIndex = 0;

		foreach (var sourceChar in source.Where(char.IsDigit))
			onlyNumbers[lastIndex++] = sourceChar;

		return new string(onlyNumbers, StartIndex, lastIndex);
	}
}