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

public class GetAllAsyncTest
{
	private readonly DbContextOptions<AppDbContext> _dbContextOptions;
	private readonly AppDbContext _mockedDbContext;

	public GetAllAsyncTest()
	{
		_dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase($"AppDatabase-{Guid.NewGuid()}").Options;
		_mockedDbContext = Create.MockedDbContextFor<AppDbContext>(_dbContextOptions);
	}

	[Test]
	public async Task GetAllAsync_ShouldInkoveToListAsyncMethodToReturnListPhysicalPersons()
	{
		var physicalPersons = new PhysicalPersonBuilder().Generate(10);
		_mockedDbContext.Set<PhysicalPerson>().AddRange(physicalPersons);
		_mockedDbContext.SaveChanges();
		var repository = new PhysicalPersonRepository(_mockedDbContext);

		var result = await repository.GetAllAsync();

		result.Should().BeEquivalentTo(physicalPersons);
	}
}