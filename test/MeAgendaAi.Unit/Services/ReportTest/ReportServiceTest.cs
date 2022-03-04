using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MeAgendaAi.Unit.Services.ReportTest
{
    public class ReportServiceTest
    {
        private readonly ReportService _reportService;

        public ReportServiceTest() => _reportService = new ReportService();

        [Test]
        public void ReportCompany_ShouldCorrectlyGenerateReport()
        {
            const int QuantityCompanies = 10;
            var companies = new CompanyBuilder().Generate(QuantityCompanies);
            var csvExpected = CsvReport(companies);

            var result = _reportService.Generate<Company, CompanyMap>(companies);

            using var stream = new MemoryStream(result);
            var reader = new StreamReader(stream);
            var csvFile = reader.ReadToEnd();
            csvFile.Should().BeEquivalentTo(csvExpected);
        }

        [Test]
        public void ReportCompany_ShouldGenerateEmptyReportWhenListOfCompaniesIsNull()
        {
            var expectedCsv = "";

            var result = _reportService.Generate<Company, CompanyMap>(null!);

            using var stream = new MemoryStream(result);
            var reader = new StreamReader(stream);
            var csvFile = reader.ReadToEnd();
            csvFile.Should().BeEquivalentTo(expectedCsv);
        }

        [Test]
        public void ReportCompany_ShouldGenerateEmptyReportWhenListOfCompaniesZero()
        {
            var expectedCsv = "";
            const int QuantityCompanies = 0;
            var companies = new CompanyBuilder().Generate(QuantityCompanies);

            var result = _reportService.Generate<Company, CompanyMap>(companies);

            using var stream = new MemoryStream(result);
            var reader = new StreamReader(stream);
            var csvFile = reader.ReadToEnd();
            csvFile.Should().BeEquivalentTo(expectedCsv);
        }

        private string CsvReport(List<Company> companies)
        {
            var header = Header();
            var body = Body(header, companies);
            return body.ToString();
        }

        private static StringBuilder Header()
        {
            var header = new StringBuilder();
            return header.Append("User Code;Name;Email;CNPJ;Description;Limit cancel hours").Append("\r\n");
        }

        private StringBuilder Body(StringBuilder header, List<Company> companies)
        {
            companies.ForEach(company =>
            {
                header.Append($"{company.Id};");
                header.Append($"{company.Name.FullName};");
                header.Append($"{company.Email.Email};");
                header.Append($"{company.CNPJ};");
                header.Append($"{company.Description};");
                header.Append($"{company.LimitCancelHours}");
                header.Append("\r\n");
            });
            header.Append("\r\n");
            return header;
        }
    }
}