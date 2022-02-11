using AutoBogus;
using Bogus;

namespace MeAgendaAi.Common.Builder.Common
{
    public static class PasswordBuilder
    {
        const int LENGTH_PASSWORD_MINIMUM = 06;
        const int LENGTH_PASSWORD_MAXIMUM = 32;
        public static string Generate(int lengthMininum = LENGTH_PASSWORD_MINIMUM, int lengthMaximium = LENGTH_PASSWORD_MAXIMUM)
        {
            var faker = new Faker();
            return faker.Internet.Password(length: faker.Random.Int(lengthMininum, lengthMaximium));
        }
    }
}
