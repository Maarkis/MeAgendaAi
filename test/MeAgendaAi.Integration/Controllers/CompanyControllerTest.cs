using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Integration.SetUp;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.Controllers
{
    public class CompanyControllerTest : TestBase
    {
        [Test]
        public async Task ReportAsync_ShouldGenerateReportAndReturnStatusCode200Ok()
        {
            var companies = new CompanyBuilder().Generate(10);
            await _dbContext.AddRangeAsync(companies);
            await _dbContext.SaveChangesAsync();

            var result = await _client.GetAsync(RequisitionAssemblyFor("Company", "Report"));

            result.Should().Be200Ok();
        }

        [Test]
        public async Task ReportAsync_ShouldNotGenerateReportAndReturnStatusCode404NotFound()
        {
            var result = await _client.GetAsync(RequisitionAssemblyFor("Company", "Report"));

            result.Should().Be404NotFound();
        }

        [Test]
        public async Task ReportAsync_ShouldGenerateReportCorrectly()
        {
            await _dbContext.AddRangeAsync(new CompanyBuilder().Generate(2));
            await _dbContext.SaveChangesAsync();
            var companies = await _dbContext.Companies.ToListAsync();
            var csvExpected = CsvReport(companies);

            var result = await _client.GetStreamAsync(RequisitionAssemblyFor("Company", "Report"));

            using var reader = new StreamReader(result);
            var csvFile = reader.ReadToEnd();            
            csvFile.Should().Be(csvExpected);
        }

        [Test]
        public async Task ReportAsync_SpecificCSVSize()
        {
            await _dbContext.AddRangeAsync(new CompanyBuilder().Generate(1));
            await _dbContext.SaveChangesAsync();
            var companies = await _dbContext.Companies.ToListAsync();
            var csvExpected = CsvReport(companies);

            var result = await _client.GetStreamAsync(RequisitionAssemblyFor("Company", "Report"));

            using var reader = new StreamReader(result);
            var csvFile = reader.ReadToEnd();
            csvFile.Length.Should().Be(csvExpected.Length);
        }

        [Test]
        public async Task ReportAsync_ShouldNotGenerateReport()
        {
            var responseExpected = new BaseMessage("Nenhuma companhia encotrada.");

            var result = await _client.GetAsync(RequisitionAssemblyFor("Company", "Report"));
            var response = await result.Content.ReadFromJsonAsync<BaseMessage>();

            response.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task ReportAsync_ShouldGenerateReportWithNameCorrectly()
        {
            await _dbContext.AddRangeAsync(new CompanyBuilder().Generate());
            await _dbContext.SaveChangesAsync();            
            var nameArchiveExpected = $"\"Report_Company_{DateTime.Now.ToShortDateString()}.csv\"";

            var result = await _client.GetAsync(RequisitionAssemblyFor("Company", "Report"));

            var headers = result.Content.Headers;
            headers.ContentDisposition?.FileName.Should().Be(nameArchiveExpected);            
        }

        private string CsvReport(List<Company> companies)
        {
            var header = Header();
            var body = Body(header, companies);
            return body.ToString();
        }
        private StringBuilder Header()
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
