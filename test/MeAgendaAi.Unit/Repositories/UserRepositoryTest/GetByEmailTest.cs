using System;
using System.Threading.Tasks;
using Bogus;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Repositories.UserRepositoryTest;

public class GetByEmailTest
{
	private readonly DbContextOptions<AppDbContext> _dbContextOptions;
	private readonly Faker _faker;
	private AppDbContext _mockedDbContext = default!;

	public GetByEmailTest()
	{
		_faker = new Faker();
		_dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase($"AppDatabase-{Guid.NewGuid()}").Options;
		CreateDbContext();
	}

	[SetUp]
	public void SetUp()
	{
		CreateDbContext();
	}

	[Test]
	public async Task GetByEmail_ShouldReturnUserWhenFindByEmail()
	{
		var emailExpected = _faker.Internet.Email();
		var users = new UserBuilder().Generate(1);
		var userExpected = new UserBuilder().WithEmail(emailExpected).Generate();
		users.Add(userExpected);
		_mockedDbContext.Set<User>().AddRange(users);
		await _mockedDbContext.SaveChangesAsync();
		var userRepository = new UserRepository(_mockedDbContext);

		var result = await userRepository.GetEmailAsync(emailExpected);

		result.Should().BeEquivalentTo(userExpected);
	}

	[Test]
	public async Task GetByEmail_ShouldReturnNullWhenNoFindUserByEmail()
	{
		var emailExpected = _faker.Internet.Email();
		var userRepository = new UserRepository(_mockedDbContext);

		var result = await userRepository.GetEmailAsync(emailExpected);

		result.Should().BeNull();
	}

	private void CreateDbContext()
	{
		_mockedDbContext = Create.MockedDbContextFor<AppDbContext>(_dbContextOptions);
	}
}