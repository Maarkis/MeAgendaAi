using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Interfaces.Repositories
{
    /// <summary>
    /// Agreement for User Repository.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Get a repository <paramref name="User"/> by e-mail.
        /// </summary>                
        /// <param name="email">E-mail of user</param>
        /// <returns>
        /// Return a user if <c>found</c>. Return <c>default( <paramref name="User"/> )</c> if not found.
        /// </returns>
        Task<User?> GetEmailAsync(string email);
    }
}

