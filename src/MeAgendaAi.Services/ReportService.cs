using CsvHelper;
using CsvHelper.Configuration;
using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Infra.Extension;
using System.Text;

namespace MeAgendaAi.Services
{
    public interface IReport
    {
        Encoding Encoding { get; }
        string Delimiter { get; }

        byte[] Generate<T, TEntityMap>(IEnumerable<T> entities) where T : Entity where TEntityMap : ClassMap<T>;

        void SetEncoding(Encoding encoding);

        void SetDelimiter(string delimiter);
    }

    public class ReportService : IReport
    {
        public Encoding Encoding { get; private set; } = Encoding.UTF8;
        public string Delimiter { get; private set; } = ";";

        public byte[] Generate<T, TEntityMap>(IEnumerable<T> entities)
            where T : Entity
            where TEntityMap : ClassMap<T>
        {
            if (entities.IsEmpty())
                return Array.Empty<byte>();

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            var config = new CsvConfiguration(Thread.CurrentThread.CurrentCulture) { Delimiter = Delimiter, Encoding = Encoding };
            using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap<TEntityMap>();
            csv.WriteRecords(entities);
            csv.NextRecord();
            return stream.ToArray();
        }

        public void SetDelimiter(string delimiter) => Delimiter = delimiter;

        public void SetEncoding(Encoding encoding) => Encoding = encoding;
    }
}