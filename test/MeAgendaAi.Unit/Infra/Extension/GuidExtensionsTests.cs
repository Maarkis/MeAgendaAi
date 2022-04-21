using System;
using FluentAssertions;
using MeAgendaAi.Infra.Extension;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.Extension;

public class GuidExtensionsTests
{
	[Test]
	public void Guid_IsEmptyShouldReturnTrueWhenGuidIsEmpty()
	{
		Guid.Empty.IsEmpty().Should().BeTrue();
	}
	
	[Test]
	public void Guid_IsEmptyShouldReturnFalseWhenGuidIsValid()
	{
		Guid.NewGuid().IsEmpty().Should().BeFalse();
	}
	
	[Test]
	public void GuidNullable_IsEmptyShouldReturnTrueWhenGuidIsEmpty()
	{
		Guid? guid = null;
		guid.IsEmpty().Should().BeTrue();
	}
	
	[Test]
	public void GuidNullable_IsEmptyShouldReturnTrueWhenGuidIsNull()
	{
		Guid? guid = Guid.Empty;
		guid.IsEmpty().Should().BeTrue();
	}
	
	[Test]
	public void GuidNullable_IsEmptyShouldReturnFalseWhenGuidIsValid()
	{
		Guid? guid = Guid.NewGuid();
		guid.IsEmpty().Should().BeFalse();
	}
}