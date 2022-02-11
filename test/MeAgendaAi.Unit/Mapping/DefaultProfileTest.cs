using AutoMapper;
using FluentAssertions;
using MeAgendaAi.Application.Mapper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Mapping
{
    public class DefaultProfileTest
    {
        private readonly Mapper _mapper;

        public DefaultProfileTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<DefaultProfile>());
            _mapper = new Mapper(configuration);
        }

        [Test]
        public void Mapper_ShouldHaveValidConfiguration() =>
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();

        [Test]
        public void Mapper_ShouldMapAddPhysicalPersonRequestToPhysicalPerson()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            var physicalPersonExpected = new PhysicalPersonBuilder().ByRequest(request).Generate();

            var physicalPerson = _mapper.Map<PhysicalPerson>(request);

            physicalPerson.Should().BeEquivalentTo(physicalPersonExpected,
                 options => options.Excluding(prop => prop.Id).Excluding(prop => prop.CreatedAt).Excluding(prop => prop.LastUpdatedAt));
        }

        [Test]
        public void Mapper_ShouldMapPhysicalPersonToPhysicalPersonResponse()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var responseExpected = new PhysicalPersonResponseBuilder().WithPhysicalPerson(physicalPerson).Generate();

            var response = _mapper.Map<PhysicalPersonResponse>(physicalPerson);

            response.Should().BeEquivalentTo(responseExpected);
        }
    }
}