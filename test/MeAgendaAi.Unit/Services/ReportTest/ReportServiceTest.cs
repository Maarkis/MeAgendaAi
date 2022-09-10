using System.Text;
using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.ReportTest;

public class ReportServiceTest
{
	private static readonly object[] EncondigCases =
	{
		new object[] { Encoding.UTF8 },
		new object[] { Encoding.UTF32 },
		new object[] { Encoding.ASCII },
		new object[] { Encoding.Latin1 }
	};

	private readonly ReportService _reportService;

	public ReportServiceTest()
	{
		_reportService = new ReportService();
	}

	[Test]
	public void ReportCompany_ShouldNotReturnNull()
	{
		const int QuantityCompanies = 10;
		var companies = new CompanyBuilder().Generate(QuantityCompanies);

		var result = _reportService.Generate<Company, CompanyMap>(companies);

		result.Should().NotBeNullOrEmpty();
	}

	[Test]
	public void ReportCompany_ShouldGenerateEmptyReportWhenListOfCompaniesIsNull()
	{
		var result = _reportService.Generate<Company, CompanyMap>(null!);

		result.Should().BeEmpty();
	}

	[Test]
	public void ReportCompany_ShouldGenerateEmptyReportWhenListOfCompaniesZero()
	{
		const int QuantityCompanies = 0;
		var companies = new CompanyBuilder().Generate(QuantityCompanies);

		var result = _reportService.Generate<Company, CompanyMap>(companies);

		result.Should().BeEmpty();
	}

	[TestCase(";")]
	[TestCase(".")]
	[TestCase("|")]
	[TestCase("-")]
	public void ReportCompany_ShouldDefineDemiliterCorrectly(string delimiter)
	{
		_reportService.SetDelimiter(delimiter);

		_reportService.Delimiter.Should().Be(delimiter);
	}

	[TestCaseSource(nameof(EncondigCases))]
	public void ReportCompany_ShouldDefineEncodeCorrectly(Encoding encoding)
	{
		_reportService.SetEncoding(encoding);

		_reportService.Encoding.Should().Be(encoding);
	}
}