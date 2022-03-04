using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class ReportCompanyServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly CompanyService _companyService;

        private const string NameKeyReport = "ReportCompany";
        private const double ExpireInSecondsReportCompany = 1200;

        private const string ActionType = "CompanyService";

        public ReportCompanyServiceTest()
        {
            _mocker = new AutoMocker();
            _companyService = _mocker.CreateInstance<CompanyService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ICompanyRepository>().Reset();
            _mocker.GetMock<IReport>().Reset();
            _mocker.GetMock<IUserService>().Reset();
            _mocker.GetMock<ILogger<CompanyService>>().Reset();
            _mocker.Get<NotificationContext>().Clear();
            _mocker.GetMock<IDistributedCacheRepository>().Reset();
        }

        [Test]
        public async Task ReportAsync_ShouldCallGetAllAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<ICompanyRepository>().Verify(verify => verify.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task ReportAsync_ShouldCallReportAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);
            _mocker.GetMock<IReport>()
                .Setup(method => method.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)))
                .Returns(csvExpected);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.Is<IEnumerable<Company>>(f => f == companies)), Times.Once);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsEmptyOfGetAllAsync()
        {
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(new List<Company>());

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsNullOfGetAllAsync()
        {
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync());

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_ShouldGenerateAnInformationLogWhenReportGeneratedSuccessfully()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync()).ReturnsAsync(companies);
            var logMessageexpected = $"[{ActionType}/ReportAsync] Report generated successfully.";

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Information, logMessageexpected);
        }

        [Test]
        public async Task ReportAsync_ShouldCallTheGetAsyncMethodOnce()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync(companies);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.GetAsync<IEnumerable<Company>>(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ReportAsync_ShouldNotCallGetAllAsyncMethodWhenReturningCachedCompanyList()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync(companies);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<ICompanyRepository>()
                .Verify(verify => verify.GetAllAsync(), Times.Never());
        }
        [Test]
        public async Task ReportAsync_ShouldNotCallSetAsyncMethodWhenReturningCachedCompanyList()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync(companies);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(It.IsAny<string>(), companies), Times.Never());
        }

        [Test]
        public async Task ReportAsync_ShouldCallSetAsyncMethodWhenNotReturningCachedListCompany()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Company>>(),
                    It.IsAny<double>()), Times.Once());
        }

        [Test]
        public async Task ReportAsync_ShouldCallGetAsyncMethodWithExpectedKey()
        {
            var keyExpected = NameKeyReport;
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(NameKeyReport));

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.GetAsync<IEnumerable<Company>>(It.Is<string>(key => key == keyExpected)), Times.Once());
        }

        [Test]
        public async Task ReportAsync_ShouldCallSetAsyncMethodWithExpectedKeyAndExpectedValue()
        {
            var keyExpected = NameKeyReport;
            var expireInSecondsExpected = ExpireInSecondsReportCompany;
            var companiesExpected = new CompanyBuilder().Generate(10);
            _mocker.GetMock<IDistributedCacheRepository>()
                .Setup(method => method.GetAsync<IEnumerable<Company>>(NameKeyReport))
                .ReturnsAsync((IEnumerable<Company>)null!);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companiesExpected);

            _ = await _companyService.ReportAsync();

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(
                    It.Is<string>(key => key == keyExpected),
                    It.Is<IEnumerable<Company>>(companyList => companyList == companiesExpected),
                    It.Is<double>(expireInSecond => expireInSecond == ExpireInSecondsReportCompany)), Times.Once());
        }
    }
}