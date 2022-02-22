using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Services;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;

namespace MeAgendaAi.Unit.Services.PhysicalPersonTest
{
    internal class GetByIdPhysicalPersonServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly PhysicalPersonService _physicalPersonService;

        public GetByIdPhysicalPersonServiceTest()
        {
            _mocker = new AutoMocker();
            _physicalPersonService = _mocker.CreateInstance<PhysicalPersonService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IPhysicalPersonRepository>().Reset();
        }

        [Test]
        public void GetById_ShouldInvokeGetByIdMethodOnce()
        {
            var physicalPersonId = Guid.NewGuid();
            var company = new PhysicalPersonBuilder().WithId(physicalPersonId).Generate();
            _mocker.GetMock<IPhysicalPersonRepository>()
                .Setup(method => method.GetByIdAsync(It.Is<Guid>(id => id == physicalPersonId)))
                .ReturnsAsync(company);

            _ = _physicalPersonService.GetByIdAsync(physicalPersonId);

            _mocker.GetMock<IPhysicalPersonRepository>()
                .Verify(verify => verify.GetByIdAsync(It.Is<Guid>(id => id == physicalPersonId)), Times.Once());
        }
    }
}