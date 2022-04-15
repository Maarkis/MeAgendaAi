using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAí.Infra.Notification;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services
{
    public class PhysicalPersonService : Service<PhysicalPerson>, IPhysicalPersonService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;
        private readonly ILogger<PhysicalPersonService> _logger;
        private const string ActionType = "PhysicalPersonService";

        public PhysicalPersonService(
            IUserService userService,
            IPhysicalPersonRepository physicalPersonRepository,
            NotificationContext notificationContext,
            ILogger<PhysicalPersonService> logger) : base(physicalPersonRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(AddPhysicalPersonRequest request)
        {
            if (await _userService.HasUser(request.Email))
            {
                _notificationContext.AddNotification("Email", "Email já cadastrado");
                _logger.LogError("[{ActionType}/AddAsync] A registered user for {Email} already exists.", ActionType, request.Email);
                return Guid.Empty;
            }

            if (_userService.NotSamePassword(request.Password, request.ConfirmPassword))
            {
                _notificationContext.AddNotification("ConfirmPassword", "Senha de confirmação não é igual a senha");
                _logger.LogError("[{ActionType}/AddAsync] Confirmation password is not the same as password.", ActionType);
                return Guid.Empty;
            }

            var physicalPerson = new PhysicalPerson(request.Email, request.Password, request.Name, request.Surname, request.CPF, request.RG);
            if (physicalPerson.Invalid)
            {
                _notificationContext.AddNotifications(physicalPerson.ValidationResult);
                _logger.LogError("[{ActionType}/AddAsync] Invalid information {Errors}", ActionType, physicalPerson.ValidationResult.Errors);
                return Guid.Empty;
            }

            physicalPerson.Encript(Encrypt.EncryptString(physicalPerson.Password, physicalPerson.Id.ToString()));

            var physicalPersonId = await AddAsync(physicalPerson);
            _logger.LogInformation("[{ActionType}/AddAsync] User {physicalPersonId} registered successfully.", ActionType, physicalPersonId);
            return physicalPersonId;
        }
    }
}