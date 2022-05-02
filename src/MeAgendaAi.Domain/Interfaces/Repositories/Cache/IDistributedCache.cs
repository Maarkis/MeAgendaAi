namespace MeAgendaAi.Domains.Interfaces.Repositories.Cache;

public interface IDistributedCacheRepository
{
	Task<T?> GetAsync<T>(string key);

	Task SetAsync<T>(string key, T value, DateTime? expireIn = null);

	Task SetAsync<T>(string key, T value, double expireInSeconds);

	Task RemoveAsync(string key);
}