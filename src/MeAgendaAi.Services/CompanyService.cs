using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.Extension;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Services.CSVMaps;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services
{
    public class CompanyService : Service<Company>, ICompanyService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;
        private readonly IReport _report;
        private readonly ILogger<CompanyService> _logger;
        private readonly IDistributedCacheRepository _distributedCacheRepository;
        private readonly IEmailService _emailService;

        private const string KeyReportCompany = "CompaniesReport";
        private const double ExpireInSecondsReportCompany = 1200;

        private const string ActionType = "CompanyService";

        public CompanyService(
            IUserService userService,
            ICompanyRepository companyRepository,
            NotificationContext notificationContext,
            IReport report, ILogger<CompanyService> logger,
            IDistributedCacheRepository distributedCacheRepository,
            IEmailService emailService) : base(companyRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
            _report = report;
            _logger = logger;
            _distributedCacheRepository = distributedCacheRepository;
            _emailService = emailService;
        }

        public async Task<Guid> AddAsync(AddCompanyRequest request)
        {
            if (await _userService.HasUser(request.Email))
            {
                _notificationContext.AddNotification("Email", "E-mail already registered.");
                _logger.LogError("[{ActionType}/AddAsync] A registered user for {Email} already exists", ActionType, request.Email);
                return Guid.Empty;
            }

            if (_userService.NotSamePassword(request.Password, request.ConfirmPassword))
            {
                _notificationContext.AddNotification("ConfirmPassword", "Confirmation password is not the same as password.");
                _logger.LogError("[{ActionType}/AddAsync] Confirmation password is not the same as password", ActionType);
                return Guid.Empty;
            }

            var company = new Company(request.Email, request.Password, request.Name, request.CNPJ, request.Description, request.LimitCancelHours);
            if (company.Invalid)
            {
                _notificationContext.AddNotifications(company.ValidationResult);
                _logger.LogError("[{ActionType}/AddAsync] Invalid information {Errors}", ActionType, company.ValidationResult.Errors);
                return Guid.Empty;
            }

            company.Encrypt(Encrypt.EncryptString(company.Password, company.Id.ToString()));

            var companyId = await AddAsync(company);
            _logger.LogInformation("[{ActionType}/AddAsync] User {CompanyId} registered successfully", ActionType, companyId);
            
            
            var sended = await _emailService.SendConfirmationEmail(company.Name.FullName, company.Email.Address, companyId.ToString());
            var notSended = !sended;

            if (notSended)
            {
                _notificationContext.AddNotification("Email", "Confirmation email not sent");
                _logger.LogError("[{ActionType}/AddAsync] User {Id} confirmation email not sent", ActionType,
                    companyId);
            }
            
            return companyId;
        }

        public async Task<byte[]?> ReportAsync()
        {
            var companies = await _distributedCacheRepository.GetAsync<IEnumerable<Company>>(KeyReportCompany);
            
            if (companies == null)
            {
                companies = await GetAllAsync();
                
                if (companies.IsEmpty()) 
                    return null;

                await _distributedCacheRepository.SetAsync(KeyReportCompany, companies, expireInSeconds: ExpireInSecondsReportCompany);
            }

            var report = _report.Generate<Company, CompanyMap>(companies);

            _logger.LogInformation("[{ActionType}/ReportAsync] Report generated successfully", ActionType);

            return report;
        }
    }
}