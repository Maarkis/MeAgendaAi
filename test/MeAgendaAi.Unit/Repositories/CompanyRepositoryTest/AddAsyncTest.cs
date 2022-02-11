using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Repositories.CompanyRepositoryTest
{
    public class AddAsyncTest
    {
        private readonly Mock<AppDbContext> _contextDb;
        private readonly Mock<DbSet<Company>> _dbSetCompany;
        public AddAsyncTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
             .UseInMemoryDatabase("AppDatabase").Options;
            _contextDb = new Mock<AppDbContext>(options);
            _dbSetCompany = new Mock<DbSet<Company>>();
        }

        [Test]
        public async Task AddAsync_ShouldInvokeAddAsyncMethodOnce()
        {
            var company = new CompanyBuilder().Generate();
            _contextDb.Setup(x => x.Set<Company>()).Returns(_dbSetCompany.Object);
            var repository = new CompanyRepository(_contextDb.Object);

            _ = await repository.AddAsync(company);

            _dbSetCompany.Verify(method => method.AddAsync(It.IsAny<Company>(), default), Times.Once);
        }

        [Test]
        public async Task AddAsync_ShouldInvokeSaveChangesAsyncMethodOnce()
        {
            var company = new CompanyBuilder().Generate();
            _contextDb.Setup(x => x.Set<Company>()).Returns(_dbSetCompany.Object);
            var repository = new CompanyRepository(_contextDb.Object);

            _ = await repository.AddAsync(company);

            _contextDb.Verify(method => method.SaveChangesAsync(default));
        }
    }
}
