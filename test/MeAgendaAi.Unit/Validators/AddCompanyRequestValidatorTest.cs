using FluentAssertions;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators
{
    public class AddCompanyRequestValidatorTest
    {
        [Test]
        public void AddCompanyRequestValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var validation = new AddCompanyRequestValidator().Validate(request);

            validation.IsValid.Should().BeTrue();
            validation.Errors.Should().BeEmpty();
        }

        [Test]
        public void AddCompanyRequestValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithNameInvalid()
                .WithEmailInvalid()
                .WithConfirmPasswordInvalid()
                .WithPasswordInvalid()
                .WithCNPJInvalid()
                .WithDescriptionInvalid()
                .Generate();

            var validation = new AddCompanyRequestValidator().Validate(requestInvalid);

            validation.IsValid.Should().BeFalse();
            validation.Errors.Should().NotBeEmpty();
        }
    }
}