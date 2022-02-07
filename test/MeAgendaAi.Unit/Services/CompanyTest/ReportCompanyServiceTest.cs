using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class ReportCompanyServiceTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ICompanyRepository> _mockCompanyRepository;
        private NotificationContext _notificationContext;
        private Mock<IReport> _mockReport;
        private Mock<ILogger<CompanyService>> _mockLogger;
        private CompanyService _companyService;

        private const string ActionType = "CompanyService";

        public ReportCompanyServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _notificationContext = new NotificationContext();
            _mockReport = new Mock<IReport>();
            _mockLogger = new Mock<ILogger<CompanyService>>();
            _companyService = new CompanyService(
                _mockUserService.Object,
                _mockCompanyRepository.Object,
                _notificationContext,
                _mockReport.Object,
                _mockLogger.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockCompanyRepository.Reset();
            _mockReport.Reset();
            _mockUserService.Reset();
            _mockLogger.Reset();
            _notificationContext.Clear();
        }

        [Test]
        public async Task ReportAsync_ShouldCallGetAllAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mockCompanyRepository
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);
            _mockReport
                .Setup(method => method.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)))
                .Returns(csvExpected);

            var result = await _companyService.ReportAsync();

            _mockCompanyRepository.Verify(verify => verify.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task ReportAsync_ShouldCallReportAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mockCompanyRepository
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);
            _mockReport
                .Setup(method => method.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)))
                .Returns(csvExpected);

            var result = await _companyService.ReportAsync();

            _mockReport.Verify(
                verify => verify.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)), Times.Once);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsEmptyOfGetAllAsync()
        {
            _mockCompanyRepository
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(new List<Company>());

            var result = await _companyService.ReportAsync();

            _mockReport.Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsNullOfGetAllAsync()
        {
            _mockCompanyRepository
                .Setup(method => method.GetAllAsync());

            var result = await _companyService.ReportAsync();

            _mockReport.Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_ShouldGenerateAnInformationLogWhenReportGeneratedSuccessfully()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mockCompanyRepository
                .Setup(method => method.GetAllAsync()).ReturnsAsync(companies);
            var logMessageexpected = $"[{ActionType}/ReportAsync] Report generated successfully.";

            var result = await _companyService.ReportAsync();

            _mockLogger.VerifyLog(LogLevel.Information, logMessageexpected);
        }
    }
}