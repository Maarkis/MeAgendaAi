using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.Extension;
using Microsoft.Extensions.Caching.Distributed;

namespace MeAgendaAi.Infra.Cache
{
    public class DistributedCacheRepository : IDistributedCacheRepository
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return GetStringAsync<T>(key);
        }

        public Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return _distributedCache.RemoveAsync(key);
        }

        public Task SetAsync<T>(string key, T value, DateTime? expireIn = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireIn,
            });
        }

        public Task SetAsync<T>(string key, T value, double expireInSeconds)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireInSeconds),
            });
        }

        private async Task<T?> GetStringAsync<T>(string key)
        {
            var source = await _distributedCache.GetStringAsync(key);

            return source == null ? default : source.Deserialize<T>();
        }

        private async Task SetStringAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            options ??= new DistributedCacheEntryOptions();

            await _distributedCache.SetStringAsync(key, value!.Serialize(), options);
        }
    }
}