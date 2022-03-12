using MeAgendaAi.Infra.Extension.Newtonsoft;
using Newtonsoft.Json;

namespace MeAgendaAi.Infra.Extension
{
    public static class ObjectExtension
    {
        public static string Serialize(this object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return JsonConvert.SerializeObject(source);
        }

        public static T? Deserialize<T>(this string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            var settingsDeserializeObject = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CustomContractResolver()
            };

            return JsonConvert.DeserializeObject<T>(source, settingsDeserializeObject);
        }
    }
}