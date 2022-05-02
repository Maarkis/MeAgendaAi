using System;
using AutoBogus;
using FluentAssertions;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.ValueObjects;
using MeAgendaAi.Infra.Extension;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.Extension;

[TestFixture(typeof(string))]
[TestFixture(typeof(User))]
[TestFixture(typeof(PhysicalPerson))]
[TestFixture(typeof(Company))]
[TestFixture(typeof(Name))]
[TestFixture(typeof(Email))]
public class ObjectExtensionsTests<TType> where TType : class
{
	[TestCase]
	public void Serialize_ShouldSerializeObjectCorrectly()
	{
		var objectType = new AutoFaker<TType>().Generate();
		var objectSerializeExpected = JsonConvert.SerializeObject(objectType);

		var objectTypeSerialize = objectType.Serialize();

		objectTypeSerialize.Should().Be(objectSerializeExpected);
	}

	[Test]
	public void Serialize_ShouldThrowArgumentNullExceptionWhenObjectIsNull()
	{
		TType objectType = null!;

		Assert.Throws<ArgumentNullException>(() => objectType.Serialize());
	}

	[Test]
	public void Serialize_ShouldThrowArgumentNullExceptionWithMessageWhenObjectIsNull()
	{
		TType objectType = null!;

		Assert.Catch<ArgumentNullException>(() => objectType.Serialize(), "Value cannot be null. (Parameter 'source')");
	}

	[Test]
	public void Deserialize_ShouldDeserializeObjectCorrectly()
	{
		var objectType = new AutoFaker<TType>().Generate();
		var objectSerialize = JsonConvert.SerializeObject(objectType);

		var stringDeserialize = objectSerialize.Deserialize<TType>();

		stringDeserialize.Should().BeEquivalentTo(objectType);
	}

	[Test]
	public void Deserialize_ShouldThrowArgumentNullExceptionWhenStringIsNull()
	{
		string objectType = null!;

		Assert.Throws<ArgumentNullException>(() => objectType.Deserialize<TType>());
	}

	[Test]
	public void Deserialize_ShouldThrowArgumentNullExceptionWithMessageWhenStringIsNull()
	{
		string objectType = null!;

		Assert.Catch<ArgumentNullException>(() => objectType.Deserialize<TType>(),
			"Value cannot be null. (Parameter 'source')");
	}
}