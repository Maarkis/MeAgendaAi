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
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class ReportCompanyServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly CompanyService _companyService;

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
        }

        [Test]
        public async Task ReportAsync_ShouldCallGetAllAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);
            _mocker.GetMock<IReport>()
                .Setup(method => method.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)))
                .Returns(csvExpected);

            var result = await _companyService.ReportAsync();

            _mocker.GetMock<ICompanyRepository>().Verify(verify => verify.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task ReportAsync_ShouldCallReportAsyncMethod()
        {
            var companies = new CompanyBuilder().Generate(10);
            var csvExpected = Array.Empty<byte>();
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(companies);
            _mocker.GetMock<IReport>()
                .Setup(method => method.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)))
                .Returns(csvExpected);

            var result = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.Is<List<Company>>(f => f == companies)), Times.Once);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsEmptyOfGetAllAsync()
        {
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync())
                .ReturnsAsync(new List<Company>());

            var result = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_NotCallGenerateWhenReturnListIsNullOfGetAllAsync()
        {
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync());

            var result = await _companyService.ReportAsync();

            _mocker.GetMock<IReport>().Verify(
                verify => verify.Generate<Company, CompanyMap>(It.IsAny<IEnumerable<Company>>()), Times.Never);
        }

        [Test]
        public async Task ReportAsync_ShouldGenerateAnInformationLogWhenReportGeneratedSuccessfully()
        {
            var companies = new CompanyBuilder().Generate(10);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetAllAsync()).ReturnsAsync(companies);
            var logMessageexpected = $"[{ActionType}/ReportAsync] Report generated successfully.";

            var result = await _companyService.ReportAsync();

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Information, logMessageexpected);
        }
    }
}