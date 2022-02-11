﻿using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Repositories
{
    public class PhysicalPersonRepositoryTest
    {
        private readonly Mock<AppDbContext> _contextDb;
        private readonly Mock<DbSet<PhysicalPerson>> _dbSetPhysicalPerson;

        public PhysicalPersonRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AppDatabase").Options;
            _contextDb = new Mock<AppDbContext>(options);
            _dbSetPhysicalPerson = new Mock<DbSet<PhysicalPerson>>();
        }

        [Test]
        public async Task PhysicalPersonRepository_ShouldInvokeAddAsyncMethodOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            _contextDb.Setup(x => x.Set<PhysicalPerson>()).Returns(_dbSetPhysicalPerson.Object);
            var repository = new PhysicalPersonRepository(_contextDb.Object);

            _ = await repository.AddAsync(physicalPerson);

            _dbSetPhysicalPerson.Verify(method => method.AddAsync(It.IsAny<PhysicalPerson>(), default), Times.Once);
        }

        [Test]
        public async Task PhysicalPersonRepository_ShouldInvokeSaveChangesAsyncMethodOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            _contextDb.Setup(x => x.Set<PhysicalPerson>()).Returns(_dbSetPhysicalPerson.Object);
            var repository = new PhysicalPersonRepository(_contextDb.Object);

            _ = await repository.AddAsync(physicalPerson);

            _contextDb.Verify(method => method.SaveChangesAsync(default));
        }
    }
}
