namespace MeAgendaAi.Domains.Interfaces.Repositories.Cache
{
    /// <summary>
    /// Agreement for Cache Repository.
    /// </summary>
    public interface IDistributedCacheRepository
    {
        /// <summary>
        /// Fetch an object from the cache repository.
        /// </summary>
        /// <typeparam name="T">The type stored by object return.</typeparam>
        /// <param name="key">Key to fetch cached repository.</param>
        /// <returns>
        ///     Returns an object from the cache repository.
        /// </returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Store an object in the cache repository.
        /// </summary>
        /// <typeparam name="T">The type stored by object return.</typeparam>
        /// <param name="key">Key of the object to be stored.</param>
        /// <param name="value">Object to be stored.</param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Store an object in the cache repository.
        /// </summary>
        /// <typeparam name="T">The type of the object to be stored.</typeparam>
        /// <param name="key">Key of the object to be stored.</param>
        /// <param name="value">Object to be stored.</param>
        /// <param name="expireIn">Object expiration of date in cache repository.</param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value, DateTime? expireIn = null);

        /// <summary>
        /// Store an object in the cache repository.
        /// </summary>
        /// <typeparam name="T">The type of the object to be stored.</typeparam>
        /// <param name="key">Key of the object to be stored.</param>
        /// <param name="value">Object to be stored.</param>
        /// <param name="expireInSeconds">Object expiration in seconds in cached repository.</param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value, double expireInSeconds);

        /// <summary>
        /// Remove an object from the cached repository.
        /// </summary>
        /// <param name="key">Object key to be removed.</param>
        /// <returns></returns>
        Task RemoveAsync(string key);
    }
}