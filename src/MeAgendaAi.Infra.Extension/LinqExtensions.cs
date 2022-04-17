namespace MeAgendaAi.Infra.Extension
{
    public static class LinqExtensions
    {
        public static bool IsEmpty<TSource>(this IEnumerable<TSource>? source)
        {
            if (source == null)
                return true;
            return !source.Any();
        }
    }
}