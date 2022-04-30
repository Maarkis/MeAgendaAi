using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.PhysicalPersonTest;

public class ReportTest
{
	private readonly AutoMocker _mocker;
	private readonly PhysicalPersonService _physicalPersonService;

	private const string ActionType = "PhysicalPersonService";
	private const string NameKeyReport = "PhysicalPersonsReport";
	private const double ExpireInSecondsReportPhysicalPersons = 1200;

	private readonly List<PhysicalPerson> _listOfPhysicalPersons;

	public ReportTest()
	{
		_mocker = new AutoMocker();
		_physicalPersonService = _mocker.CreateSelfMock<PhysicalPersonService>();
		_listOfPhysicalPersons = new PhysicalPersonBuilder().Generate(10);
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IDistributedCacheRepository>().Reset();
		_mocker.GetMock<IPhysicalPersonRepository>().Reset();
		_mocker.GetMock<IReport>().Reset();
		_mocker.GetMock<ILogger<PhysicalPersonService>>().Reset();
	}

	[Test]
	public async Task ReportAsync_ShouldCallTheGetAsyncMethodWithKeyCorrectly()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.Is<string>(prop => prop == NameKeyReport)))
			.ReturnsAsync(_listOfPhysicalPersons);

		await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify =>
					verify.GetAsync<IEnumerable<PhysicalPerson>>(It.Is<string>(prop => prop == NameKeyReport)),
				Times.Once());
	}

	[Test]
	public async Task ReportAsync_ShouldCallTheGetAsyncMethodOfDistributedRepositoryOnce()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync(_listOfPhysicalPersons);

		await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify =>
				verify.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()), Times.Once());
	}

	[Test]
	public async Task ReportAsync_ShouldNotCallGetAllAsyncMethodWhenReturningCachedPhysicalPersonsList()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.Is<string>(prop => prop == NameKeyReport)))
			.ReturnsAsync(_listOfPhysicalPersons);

		await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IPhysicalPersonRepository>()
			.Verify(verify => verify.GetAllAsync(), Times.Never());
	}

	[Test]
	public async Task ReportAsync_ShouldCallGetAllAsyncWhenThereIsNoCachedPhysicalPersonsList()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(_listOfPhysicalPersons);

		await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IPhysicalPersonRepository>()
			.Verify(verify =>
				verify.GetAllAsync(), Times.Once());
	}

	[Test]
	public async Task ReportAsync_ShouldNotCallSetAsyncMethodWhenThereIsNoPhysicalPersonsList()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);

		_ = await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify =>
				verify.SetAsync(
					It.IsAny<string>(),
					It.IsAny<IEnumerable<PhysicalPerson>>(),
					It.IsAny<double>()), Times.Never());
	}

	[Test]
	public async Task ReportAsync_ShouldCallSetAsyncMethodWhenThereIsNoCachedPhysicalPersonsList()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(_listOfPhysicalPersons);

		await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.SetAsync(
				It.IsAny<string>(),
				It.IsAny<IEnumerable<PhysicalPerson>>(),
				It.IsAny<double>()), Times.Once());
	}

	[Test]
	public async Task ReportAsync_ShouldCallSetAsyncMethodWithExpectedKeyAndExpectedValue()
	{
		var keyExpected = NameKeyReport;
		var expireInSecondsExpected = ExpireInSecondsReportPhysicalPersons;
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(_listOfPhysicalPersons);

		_ = await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.SetAsync(
				It.Is<string>(key => key == keyExpected),
				It.Is<IEnumerable<PhysicalPerson>>(physicalPersonsList =>
					Equals(physicalPersonsList, _listOfPhysicalPersons)),
				It.Is<double>(expireInSecond => expireInSecond == expireInSecondsExpected)), Times.Once());
	}

	[Test]
	public async Task ReportAsync_NotCallGenerateWhenReturnListIsEmptyOfGetAllAsync()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(new List<PhysicalPerson>());

		_ = await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IReport>().Verify(
			verify => verify.Generate<PhysicalPerson, PhysicalPersonsMap>(It.IsAny<IEnumerable<PhysicalPerson>>()),
			Times.Never);
	}


	[Test]
	public async Task ReportAsync_ShouldCallReportAsyncMethod()
	{
		var csvExpected = Array.Empty<byte>();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(_listOfPhysicalPersons);
		_mocker.GetMock<IReport>()
			.Setup(method =>
				method.Generate<PhysicalPerson, PhysicalPersonsMap>(
					It.Is<List<PhysicalPerson>>(f => f == _listOfPhysicalPersons)))
			.Returns(csvExpected);

		_ = await _physicalPersonService.ReportAsync();

		_mocker.GetMock<IReport>().Verify(
			verify => verify.Generate<PhysicalPerson, PhysicalPersonsMap>(
				It.Is<IEnumerable<PhysicalPerson>>(f => Equals(f, _listOfPhysicalPersons))),
			Times.Once);
	}

	[Test]
	public async Task ReportAsync_ShouldGenerateAnInformationLogWhenReportGeneratedSuccessfully()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<IEnumerable<PhysicalPerson>>(It.IsAny<string>()))
			.ReturnsAsync((IEnumerable<PhysicalPerson>)null!);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.GetAllAsync())
			.ReturnsAsync(_listOfPhysicalPersons);
		const string logMessageExpected = $"[{ActionType}/ReportAsync] Report generated successfully";

		_ = await _physicalPersonService.ReportAsync();

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Information, logMessageExpected);
	}
}