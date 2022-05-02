﻿using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MeAgendaAi.Infra.Extension.Newtonsoft;

public class CustomContractResolver : DefaultContractResolver
{
	protected override JsonProperty CreateProperty(
		MemberInfo member,
		MemberSerialization memberSerialization)
	{
		var prop = base.CreateProperty(member, memberSerialization);

		if (!prop.Writable)
		{
			var property = member as PropertyInfo;
			if (property != null)
			{
				var hasNonPublicSetter = property.GetSetMethod(true) != null;
				prop.Writable = hasNonPublicSetter;
			}
		}

		return prop;
	}
}