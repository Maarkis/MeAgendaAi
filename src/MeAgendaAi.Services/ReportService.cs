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
        byte[] Generate<T, EntityMap>(IEnumerable<T> entities) where T : Entity where EntityMap : ClassMap<T>;
        void SetEncoding(Encoding encoding);
        void SetDelimiter(string delimiter);
    }

    public class ReportService : IReport
    {
        private Encoding _encoding = Encoding.UTF8;
        public Encoding Encoding => _encoding;

        private string _delimiter = ";";
        public string Delimiter => _delimiter;        

        public byte[] Generate<T, EntityMap>(IEnumerable<T> entities)
            where T : Entity
            where EntityMap : ClassMap<T>
        {
            if (entities.IsEmpty())
                return Array.Empty<byte>();

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            var config = new CsvConfiguration(Thread.CurrentThread.CurrentCulture) { Delimiter = Delimiter, Encoding = Encoding };
            using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap<EntityMap>();
            csv.WriteRecords(entities);
            csv.NextRecord();
            return stream.ToArray();
        }

        public void SetDelimiter(string delimiter) => _delimiter = delimiter;
        public void SetEncoding(Encoding enconding) => _encoding = enconding;
    }
}
