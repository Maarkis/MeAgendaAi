using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    public interface ICompanyService : IService<Company>
    {
        Task<Guid> AddAsync(AddCompanyRequest request);
        Task<byte[]?> ReportAsync();
    }
}
