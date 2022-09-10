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

namespace MeAgendaAi.Unit.Repositories.CompanyRepositoryTest;

public class GetById
{
	private readonly DbContextOptions<AppDbContext> _dbContextOptions;
	private readonly AppDbContext _mockedDbContext;

	public GetById()
	{
		_dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase($"AppDatabase-{Guid.NewGuid()}").Options;
		_mockedDbContext = Create.MockedDbContextFor<AppDbContext>(_dbContextOptions);
	}

	[Test]
	public async Task GetById_ShouldReturnCompanyWhenFindById()
	{
		var id = Guid.NewGuid();
		var companies = new CompanyBuilder().Generate(10);
		var companyExpected = new CompanyBuilder().WithId(id).Generate();
		_mockedDbContext.Set<Company>().AddRange(companies);
		_mockedDbContext.Set<Company>().Add(companyExpected);
		await _mockedDbContext.SaveChangesAsync();
		var repository = new CompanyRepository(_mockedDbContext);

		var result = await repository.GetByIdAsync(id);

		result.Should().BeEquivalentTo(companyExpected);
	}

	[Test]
	public async Task GetById_ShouldReturnNullWhenFindByIdCompanyNotFound()
	{
		var companies = new CompanyBuilder().Generate(10);
		_mockedDbContext.Set<Company>().AddRange(companies);
		_mockedDbContext.SaveChanges();
		var repository = new CompanyRepository(_mockedDbContext);

		var result = await repository.GetByIdAsync(Guid.NewGuid());

		result.Should().BeNull();
	}
}