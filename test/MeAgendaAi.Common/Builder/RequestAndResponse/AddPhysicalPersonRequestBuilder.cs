using AutoBogus;
using Bogus;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse
{
    public class AddPhysicalPersonRequestBuilder : AutoFaker<AddPhysicalPersonRequest>
    {
        public AddPhysicalPersonRequestBuilder()
        {
            var password = PasswordBuilder.Generate();
            RuleFor(prop => prop.Name, faker => faker.Name.FirstName());
            RuleFor(prop => prop.Email, faker => faker.Internet.Email());
            RuleFor(prop => prop.Password, () => password);
            RuleFor(prop => prop.ConfirmPassword, () => password);

            RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
            RuleFor(prop => prop.CPF, faker => faker.Random.Int(11).ToString());
            RuleFor(prop => prop.RG, faker => faker.Random.Int(9).ToString());
        }
    }

    public static class AddPhysicalPersonRequestBuilderExtensions
    {
        public static AddPhysicalPersonRequestBuilder WithName(this AddPhysicalPersonRequestBuilder builder, string name)
        {
            builder.RuleFor(prop => prop.Name, () => name);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithSurname(this AddPhysicalPersonRequestBuilder builder, string surname)
        {
            builder.RuleFor(prop => prop.Surname, () => surname);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithEmail(this AddPhysicalPersonRequestBuilder builder, string email)
        {
            builder.RuleFor(prop => prop.Email, () => email);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithNameInvalid(this AddPhysicalPersonRequestBuilder builder, int length = 0)
        {
            var name = new Faker().Random.String(length);
            builder.WithName(name);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithSurnameInvalid(this AddPhysicalPersonRequestBuilder builder, int length = 0)
        {
            var surname = new Faker().Random.String(length);
            builder.WithSurname(surname);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithEmailInvalid(this AddPhysicalPersonRequestBuilder builder, string email = "")
        {
            builder.WithEmail(email);
            return builder;
        }
        public static AddPhysicalPersonRequestBuilder WithConfirmPassword(this AddPhysicalPersonRequestBuilder builder, string confirmPassword = "")
        {
            builder.RuleFor(prop => prop.ConfirmPassword, () => confirmPassword);
            return builder;
        }
    }
}
