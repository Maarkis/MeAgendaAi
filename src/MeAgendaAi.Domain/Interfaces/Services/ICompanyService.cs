using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    /// <summary>
    /// Agreement for company service.
    /// </summary>
    public interface ICompanyService : IService<Company>
    {
        /// <summary>
        /// Add company.
        /// </summary>
        /// <param name="request">Object with information for adding a company.</param>
        /// <returns>
        /// Return <c>Id</c> of company.
        /// </returns>
        Task<Guid> AddAsync(AddCompanyRequest request);

        /// <summary>
        /// Generate company report.
        /// </summary>
        /// <returns>
        /// Return the company report in byte array.
        /// </returns>
        Task<byte[]?> ReportAsync();
    }
}