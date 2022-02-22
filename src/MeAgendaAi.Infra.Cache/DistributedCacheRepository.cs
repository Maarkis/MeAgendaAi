using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MeAgendaAi.Infra.Cache.Repository
{
    public class DistributedCacheRepository : IDistributedCacheRepository
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var source = await _distributedCache.GetStringAsync(key);

            if (source == null)
                return default;

            return  Deserialize<T>(source);
        }

        public Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return _distributedCache.RemoveAsync(key);
        }

        public async Task SetAsync<T>(string key, T value) => await SetStringAsync(key, value);

        public async Task SetAsync<T>(string key, T value, DateTime? expireIn = null)
        {
            await SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireIn,
            });
        }
        public async Task SetAsync<T>(string key, T value, double expireInSeconds)
        {
            await SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireInSeconds),
            });
        }

        private async Task SetStringAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            options ??= new DistributedCacheEntryOptions();

            await _distributedCache.SetStringAsync(key, Serialize(value), options);
        }

        private static string Serialize<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return JsonConvert.SerializeObject(value);
        }

        private static T? Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}