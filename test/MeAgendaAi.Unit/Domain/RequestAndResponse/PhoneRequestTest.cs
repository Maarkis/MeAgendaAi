using System.Collections.Generic;
using FluentAssertions;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain.RequestAndResponse;

public class PhoneRequestTest
{
	[Test]
	public void ShouldConvertPhoneRequestToPhoneNumber()
	{
		var phoneRequest = new PhoneRequestBuilder().Generate();
		var phoneNumberExpected = new PhoneNumber(phoneRequest.CountryCode, phoneRequest.DialCode, phoneRequest.Phone,
			phoneRequest.Type, phoneRequest.Contact);

		var phoneNumber = phoneRequest.ToPhoneNumber();

		phoneNumber.Should().BeEquivalentTo(phoneNumberExpected,
			config => config.Excluding(prop => prop.CreatedAt).Excluding(prop => prop.Id));
	}
}

public class PhoneRequestExtensionsTest
{
	[Test]
	public void ShouldConvertListPhoneRequestToListPhoneNumber()
	{
		var phoneRequestOne = new PhoneRequestBuilder().Generate();
		var phoneRequestTwo = new PhoneRequestBuilder().Generate();
		var phoneRequest = new List<PhoneRequest> { phoneRequestOne, phoneRequestTwo };
		var phoneNumberExpected = new List<PhoneNumber>()
		{
			new(phoneRequestOne.CountryCode, phoneRequestOne.DialCode, phoneRequestOne.Phone, phoneRequestOne.Type,
				phoneRequestOne.Contact),
			new(phoneRequestTwo.CountryCode, phoneRequestTwo.DialCode, phoneRequestTwo.Phone, phoneRequestTwo.Type,
				phoneRequestTwo.Contact)
		};

		var phoneNumber = phoneRequest.ToPhoneNumbers();

		phoneNumber.Should().BeEquivalentTo(phoneNumberExpected,
			config => config.Excluding(prop => prop.CreatedAt).Excluding(prop => prop.Id));
	}
}