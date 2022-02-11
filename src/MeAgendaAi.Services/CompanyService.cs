using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Services
{

    public class CompanyService : Service<Company>, ICompanyService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;

        public CompanyService(IUserService userService, ICompanyRepository companyRepository, NotificationContext notificationContext) : base(companyRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
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

            return await AddAsync(company);
        }
    }
}
