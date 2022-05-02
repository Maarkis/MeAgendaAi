using System;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Repositories.PhysicalPersonRepositoryTest;

public class GetByIdTest
{
	private readonly DbContextOptions<AppDbContext> _dbContextOptions;
	private readonly AppDbContext _mockedDbContext;

	public GetByIdTest()
	{
		_dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase($"AppDatabase-{Guid.NewGuid()}").Options;
		_mockedDbContext = Create.MockedDbContextFor<AppDbContext>(_dbContextOptions);
	}

	[Test]
	public async Task GetById_ShouldReturnPhysicalPersonWhenFindById()
	{
		var id = Guid.NewGuid();
		var physicalPersons = new PhysicalPersonBuilder().Generate(10);
		var physicalPersonExpected = new PhysicalPersonBuilder().WithId(id).Generate();
		_mockedDbContext.Set<PhysicalPerson>().AddRange(physicalPersons);
		_mockedDbContext.Set<PhysicalPerson>().Add(physicalPersonExpected);
		_mockedDbContext.SaveChanges();
		var repository = new PhysicalPersonRepository(_mockedDbContext);

		var result = await repository.GetByIdAsync(id);

		result.Should().BeEquivalentTo(physicalPersonExpected);
	}

	[Test]
	public async Task GetById_ShouldReturnNullWhenFindByIdPhysicalPersonNotFound()
	{
		var companies = new PhysicalPersonBuilder().Generate(10);
		_mockedDbContext.Set<PhysicalPerson>().AddRange(companies);
		await _mockedDbContext.SaveChangesAsync();
		var repository = new PhysicalPersonRepository(_mockedDbContext);

		var result = await repository.GetByIdAsync(Guid.NewGuid());

		result.Should().BeNull();
	}
}