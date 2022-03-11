using AutoBogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Infra.JWT;
using Microsoft.Extensions.Options;
using Moq.AutoMock;
using NUnit.Framework;
using System;

namespace MeAgendaAi.Unit.Infra.Jwt
{
    public class JWTServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly TokenConfiguration _tokenConfiguration;
        private readonly JWTService _jwtService;

        public JWTServiceTest()
        {
            _mocker = new AutoMocker();
            _tokenConfiguration = new AutoFaker<TokenConfiguration>()
                .RuleFor(prop => prop.ExpirationTimeInSeconds, faker => faker.Random.Int(min: 1, max: 9999))
                .RuleFor(prop => prop.RefreshTokenExpirationTimeInSeconds, faker => faker.Random.Int(min: 1, max: 9999))
                .Generate();
            _mocker.GetMock<IOptions<TokenConfiguration>>()
                .Setup(setup => setup.Value)
                .Returns(_tokenConfiguration);
            _jwtService = _mocker.CreateInstance<JWTService>();
        }

        [Test]
        public void GenerateToken_ShouldBeOfTypeJWTToken()
        {
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();

            var token = _jwtService.GenerateToken(user);

            token.Should().BeOfType<JWTToken>();
        }

        [Test]
        public void GenerateToken_ShouldTokenNotNullOrEmpty()
        {
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();

            var token = _jwtService.GenerateToken(user);

            token.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GenerateToken_ShouldRefreshTokenNotNull()
        {

            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();

            var token = _jwtService.GenerateToken(user);

            var refreshToken = token.RefreshToken;
            refreshToken.Should().NotBeNull();
        }

        [Test]
        public void GenerateToken_RefreshTokenShouldContainTokenNotNullOrEmpty()
        {
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();

            var token = _jwtService.GenerateToken(user);

            var refreshToken = token.RefreshToken;
            refreshToken.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GenerateToken_RefreshTokenShouldContainsACurrentDatePlusTheRefreshTokenExpirationTimeinSecondsSetting()
        {
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();
            var expireInExpected = DateTime.Now.AddSeconds(_tokenConfiguration.RefreshTokenExpirationTimeInSeconds);

            var token = _jwtService.GenerateToken(user);

            var refreshToken = token.RefreshToken;
            refreshToken.ExpiresIn.Should().BeCloseTo(expireInExpected, precision: 1.Seconds());
        }
    }
}