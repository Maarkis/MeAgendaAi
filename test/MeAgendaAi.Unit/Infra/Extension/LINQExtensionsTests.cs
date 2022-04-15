using AutoBogus;
using FluentAssertions;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.ValueObjects;
using MeAgendaAi.Infra.Extension;
using NUnit.Framework;
using System.Collections.Generic;

namespace MeAgendaAi.Unit.Infra.Extension
{
    [TestFixture(typeof(string))]
    [TestFixture(typeof(User))]
    [TestFixture(typeof(PhysicalPerson))]
    [TestFixture(typeof(Company))]
    [TestFixture(typeof(Name))]
    [TestFixture(typeof(Email))]
    public class LinqExtensionsTests<TType> where TType : class
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void IsEmpty_ShouldReturnFalseWhenTheListHasOneOrMoreItems(int quantity)
        {
            var source = CreateList(quantity);

            source.IsEmpty().Should().BeFalse();
        }

        [Test]
        public void IsEmpty_ShouldReturnTrueWhenTheListIsEmpty()
        {
            var source = CreateList(0);

            source.IsEmpty().Should().BeTrue();
        }

        [Test]
        public void IsEmpty_ShouldReturnTrueWhenTheListIsNull()
        {
            List<TType> source = null!;

            source.IsEmpty().Should().BeTrue();
        }

        private static List<TType> CreateList(int quantity) => new AutoFaker<TType>().Generate(quantity);
    }
}