using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Integration.SetUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MeAgendaAi.Integration.Controllers.PhysicalPersonController;

public class ReportTest : TestBase
{
	protected override string EntryPoint => "PhysicalPerson";
	private string Url => UrlApi.AppendPathSegment(EntryPoint).AppendPathSegment("Report");

	private const string KeyReportCompany = "PhysicalPersonsReport";

	[Test]
	public async Task ReportAsync_ShouldReturnStatusCode200Ok()
	{
		var physicalPersons = new PhysicalPersonBuilder().Generate(2);
		await DbContext.PhysicalPersons.AddRangeAsync(physicalPersons);
		await DbContext.SaveChangesAsync();
		var result = await Client.GetAsync(Url);

		result.Should().Be200Ok();
	}

	[Test]
	public async Task ReportAsync_ShouldGenerateReportWithNameCorrectly()
	{
		var physicalPersons = new PhysicalPersonBuilder().Generate(2);
		await DbContext.PhysicalPersons.AddRangeAsync(physicalPersons);
		await DbContext.SaveChangesAsync();
		var nameArchiveExpected = $"\"Report_PhysicalPerson_{DateTime.Now.ToShortDateString()}.csv\"";
		const string contentTypeExpected = "csv/text";

		var result = await Client.GetAsync(Url);

		var headers = result.Content.Headers;
		headers.ContentDisposition?.FileName.Should().Be(nameArchiveExpected);
		headers.ContentType?.MediaType.Should().Be(contentTypeExpected);
	}

	[Test]
	public async Task ReportAsync_ShouldNotGenerateReportAndReturnStatusCode404NotFound()
	{
		var result = await Client.GetAsync(RequisitionAssemblyFor(EntryPoint, "Report"));

		result.Should().Be404NotFound();
	}

	[Test]
	public async Task ReportAsync_ShouldNotGenerateReport()
	{
		var responseExpected = new BaseMessage("No physical persons found.");

		var result = await Client.GetAsync(RequisitionAssemblyFor(EntryPoint, "Report"));
		var response = await result.Content.ReadFromJsonAsync<BaseMessage>();

		response.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task ReportAsync_ShouldGenerateReportCorrectly()
	{
		await DbContext.AddRangeAsync(new PhysicalPersonBuilder().Generate(2));
		await DbContext.SaveChangesAsync();
		var physicalPersons = await DbContext.PhysicalPersons.ToListAsync();
		var csvExpected = CsvReport(physicalPersons);

		var result = await Client.GetStreamAsync(RequisitionAssemblyFor(EntryPoint, "Report"));

		using var reader = new StreamReader(result);
		var csvFile = await reader.ReadToEndAsync();
		csvFile.Should().Be(csvExpected);
	}

	[Test]
	public async Task ReportAsync_SpecificCSVSize()
	{
		await DbContext.AddRangeAsync(new PhysicalPersonBuilder().Generate(2));
		await DbContext.SaveChangesAsync();
		var physicalPersons = await DbContext.PhysicalPersons.ToListAsync();
		var csvExpected = CsvReport(physicalPersons);

		var result = await Client.GetStreamAsync(RequisitionAssemblyFor(EntryPoint, "Report"));

		using var reader = new StreamReader(result);
		var csvFile = await reader.ReadToEndAsync();
		csvFile.Length.Should().Be(csvExpected.Length);
	}

	[Test]
	public async Task ReportAsync_ShouldGenerateReportCorrectlyGettingByCache()
	{
		var physicalPersons = new PhysicalPersonBuilder().Generate(1);
		var physicalPersonsSerialized = JsonConvert.SerializeObject(physicalPersons);
		await DbRedis.SetStringAsync(KeyReportCompany, physicalPersonsSerialized);
		var csvExpected = CsvReport(physicalPersons);

		var result = await Client.GetStreamAsync(RequisitionAssemblyFor(EntryPoint, "Report"));

		using var reader = new StreamReader(result);
		var csvFile = await reader.ReadToEndAsync();
		csvFile.Should().Be(csvExpected);
	}

	private static string CsvReport(List<PhysicalPerson> physicalPersons)
	{
		var header = Header();
		var body = Body(header, physicalPersons);
		return body.ToString();
	}

	private static StringBuilder Header()
	{
		var header = new StringBuilder();
		return header.Append("User Code;Name;Email;Rg;Cpf;Active").Append("\r\n");
	}

	private static StringBuilder Body(StringBuilder header, List<PhysicalPerson> companies)
	{
		companies.ForEach(physicalPerson =>
		{
			header.Append($"{physicalPerson.Id};");
			header.Append($"{physicalPerson.Name.FullName};");
			header.Append($"{physicalPerson.Email.Address};");
			header.Append($"{physicalPerson.Rg};");
			header.Append($"{physicalPerson.Cpf};");
			header.Append($"{physicalPerson.IsActive}");
			header.Append("\r\n");
		});
		header.Append("\r\n");
		return header;
	}
}