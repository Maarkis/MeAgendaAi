using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using MeAgendaAi.Services.CSVMaps;
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
        private CompanyService _companyService;

        public ReportCompanyServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _notificationContext = new NotificationContext();
            _mockReport = new Mock<IReport>();
            _companyService = new CompanyService(_mockUserService.Object, _mockCompanyRepository.Object, _notificationContext, _mockReport.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockCompanyRepository.Reset();
            _mockReport.Reset();
            _mockUserService.Reset();
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
    }

}
