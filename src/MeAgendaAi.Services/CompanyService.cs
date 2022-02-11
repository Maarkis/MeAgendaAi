using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.Extension;
using MeAgendaAi.Services.CSVMaps;
using System.Net;

namespace MeAgendaAi.Services
{

    public class CompanyService : Service<Company>, ICompanyService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;
        private readonly IReport _report;

        public CompanyService(
            IUserService userService,
            ICompanyRepository companyRepository,
            NotificationContext notificationContext,
            IReport report) : base(companyRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
            _report = report;
        }

        public async Task<Guid> AddAsync(AddCompanyRequest request)
        {
            if (await _userService.HasUser(request.Email))
            {
                _notificationContext.AddNotification("Email", "Email já cadastrado");
                return Guid.Empty;
            }

            if (_userService.NotSamePassword(request.Password, request.ConfirmPassword))
            {
                _notificationContext.AddNotification("ConfirmPassword", "Senha de confirmação não é igual a senha");
                return Guid.Empty;
            }

            var company = new Company(request.Email, request.Password, request.Name, request.CNPJ, request.Description, request.LimitCancelHours);
            if (company.Invalid)
            {
                _notificationContext.AddNotifications(company.ValidationResult);
                return Guid.Empty;
            }

            company.Encript(Encrypt.EncryptString(company.Password, company.Id.ToString()));

            return await AddAsync(company);
        }

        public async Task<byte[]?> ReportAsync()
        {
            var companies = await GetAllAsync();
            if (companies.IsEmpty())
                return null;

            return _report.Generate<Company, CompanyMap>(companies);
        }
    }
}
