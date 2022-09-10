namespace MeAgendaAi.Infra.Extension;

public static class GuidExtensions
{
	/// <summary>A GUID? extension method that query if 'source' is empty.</summary>
	/// <param name="source">The source to act on.</param>
	/// <returns>true if empty, false if not.</returns>
	public static bool IsEmpty(this Guid? source)
	{
		return !source.HasValue || IsEmpty(source.Value);
	}

	/// <summary>A GUID extension method that query if 'source' is empty.</summary>
	/// <param name="source">The source to act on.</param>
	/// <returns>true if empty, false if not.</returns>
	public static bool IsEmpty(this Guid source)
	{
		return source == Guid.Empty;
	}
}