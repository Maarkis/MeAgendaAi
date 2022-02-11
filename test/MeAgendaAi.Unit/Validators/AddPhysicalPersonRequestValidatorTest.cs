using FluentAssertions;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators
{
    public class AddPhysicalPersonRequestValidatorTest
    {
        [Test]
        public void AddPhysicalPersonValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var validation = new AddPhysicalPersonRequestValidator().Validate(request);

            validation.IsValid.Should().BeTrue();
            validation.Errors.Should().BeEmpty();
        }

        [Test]
        public void AddPhysicalPersonValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
                .WithNameInvalid()
                .WithEmailInvalid()
                .WithSurnameInvalid()
                .WithConfirmPasswordInvalid()
                .WithPasswordInvalid()
                .WithCPFInvalid()
                .WithRGInvalid()
                .Generate();

            var validation = new AddPhysicalPersonRequestValidator().Validate(requestInvalid);

            validation.IsValid.Should().BeFalse();
            validation.Errors.Should().NotBeEmpty();
        }
    }
}
