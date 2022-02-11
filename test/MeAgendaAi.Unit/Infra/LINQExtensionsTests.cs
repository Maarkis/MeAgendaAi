using AutoBogus;
using FluentAssertions;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.ValueObjects;
using MeAgendaAi.Infra.Extension;
using NUnit.Framework;
using System.Collections.Generic;

namespace MeAgendaAi.Unit.Infra
{

    [TestFixture]
    public class LINQExtensionsTests
    {
        public static IEnumerable<IGenericTestCase> TypeCases()
        {
            yield return new GenericTestClass<string>();
            yield return new GenericTestClass<User>();
            yield return new GenericTestClass<PhysicalPerson>();
            yield return new GenericTestClass<Company>();
            yield return new GenericTestClass<EmailObject>();
            yield return new GenericTestClass<NameObject>();
        }

        [Test]
        [TestCaseSource("TypeCases")]
        public void ShouldReturnFasleWhenTheListHasOneOrMoreItems(IGenericTestCase genericTestCase) =>
            genericTestCase.ShouldReturnFasleWhenTheListHasOneOrMoreItems();


        [Test]
        [TestCaseSource("TypeCases")]
        public void ShouldReturnTrueWhenTheListIsEmpty(IGenericTestCase genericTestCase) =>
            genericTestCase.ShouldReturnTrueWhenTheListIsEmpty();


        [Test]
        [TestCaseSource("TypeCases")]
        public void ShouldReturnTrueWhenTheListIsNull(IGenericTestCase genericTestCase) =>
            genericTestCase.ShouldReturnTrueWhenTheListIsNull();

    }

    public interface IGenericTestCase
    {
        void ShouldReturnFasleWhenTheListHasOneOrMoreItems();
        void ShouldReturnTrueWhenTheListIsEmpty();
        void ShouldReturnTrueWhenTheListIsNull();
    }

    public class GenericTestClass<TType> : IGenericTestCase where TType : class
    {
        public void ShouldReturnFasleWhenTheListHasOneOrMoreItems()
        {
            var source = CreateList(10);
            source.IsEmpty().Should().BeFalse();
        }

        public void ShouldReturnTrueWhenTheListIsNull()
        {
            List<TType> source = null!;
            source.IsEmpty().Should().BeTrue();
        }

        public void ShouldReturnTrueWhenTheListIsEmpty()
        {
            var source = CreateList(0);
            source.IsEmpty().Should().BeTrue();
        }

        private List<TType> CreateList(int quantity) => new AutoFaker<TType>().Generate(quantity);
    }
}
