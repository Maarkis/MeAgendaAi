using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using MeAgendaAi.Common;
using FluentAssertions;

namespace MeAgendaAi.Unit.Repositories.CompanyRepositoryTest
{
    public class GetAllAsyncTest
    {
        private readonly Mock<AppDbContext> _contextDb;
        public GetAllAsyncTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase("AppDatabase").Options;
            _contextDb = new Mock<AppDbContext>(options);            
        }

        [Test]
        public async Task GetAllAsync_ShouldInkoveToListAsyncMethodToReturnListOfCompanies()
        {
            var companies = new CompanyBuilder().Generate(10);
            var dbSetCompany = new Mock<DbSet<Company>>().MockIQueryable(companies);
            _contextDb.Setup(method => method.Set<Company>()).Returns(dbSetCompany.Object);
            var repository = new CompanyRepository(_contextDb.Object);

            var result = await repository.GetAllAsync();

            result.Should().BeEquivalentTo(companies);
        }
    }
}
