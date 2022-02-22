using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Services;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class GetByIdCompanyServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly CompanyService _companyService;

        public GetByIdCompanyServiceTest()
        {
            _mocker = new AutoMocker();
            _companyService = _mocker.CreateInstance<CompanyService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ICompanyRepository>().Reset();
        }

        [Test]
        public void GetById_ShouldInvokeGetByIdMethodOnce()
        {
            var companyId = Guid.NewGuid();
            var company = new CompanyBuilder().WithId(companyId).Generate();
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.GetByIdAsync(It.Is<Guid>(id => id == companyId)))
                .ReturnsAsync(company);

            _ = _companyService.GetByIdAsync(companyId);

            _mocker.GetMock<ICompanyRepository>()
                .Verify(verify => verify.GetByIdAsync(It.Is<Guid>(id => id == companyId)), Times.Once());
        }
    }
}