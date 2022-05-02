using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services;

public class PhysicalPersonService : Service<PhysicalPerson>, IPhysicalPersonService
{
	private const string ActionType = "PhysicalPersonService";
	private readonly IEmailService _emailService;
	private readonly ILogger<PhysicalPersonService> _logger;
	private readonly NotificationContext _notificationContext;
	private readonly IUserService _userService;

	public PhysicalPersonService(
		IUserService userService,
		IPhysicalPersonRepository physicalPersonRepository,
		NotificationContext notificationContext,
		ILogger<PhysicalPersonService> logger,
		IEmailService emailService) : base(physicalPersonRepository)
	{
		_userService = userService;
		_notificationContext = notificationContext;
		_logger = logger;
		_emailService = emailService;
	}

	public async Task<Guid> AddAsync(AddPhysicalPersonRequest request)
	{
		if (await _userService.HasUser(request.Email))
		{
			_notificationContext.AddNotification("Email", "Email já cadastrado");
			_logger.LogError("[{ActionType}/AddAsync] A registered user for {Email} already exists", ActionType,
				request.Email);
			return Guid.Empty;
		}

		if (_userService.NotSamePassword(request.Password, request.ConfirmPassword))
		{
			_notificationContext.AddNotification("ConfirmPassword", "Senha de confirmação não é igual a senha");
			_logger.LogError("[{ActionType}/AddAsync] Confirmation password is not the same as password", ActionType);
			return Guid.Empty;
		}

		var physicalPerson = new PhysicalPerson(request.Email, request.Password, request.Name, request.Surname,
			request.CPF, request.RG);
		if (physicalPerson.Invalid)
		{
			_notificationContext.AddNotifications(physicalPerson.ValidationResult);
			_logger.LogError("[{ActionType}/AddAsync] Invalid information {Errors}", ActionType,
				physicalPerson.ValidationResult.Errors);
			return Guid.Empty;
		}

		physicalPerson.Encrypt(Encrypt.EncryptString(physicalPerson.Password, physicalPerson.Id.ToString()));

		var physicalPersonId = await AddAsync(physicalPerson);
		_logger.LogInformation("[{ActionType}/AddAsync] User {PhysicalPersonId} registered successfully", ActionType,
			physicalPersonId);

		var sended = await _emailService.SendConfirmationEmail(physicalPerson.Name.FullName,
			physicalPerson.Email.Address, physicalPersonId.ToString());
		var notSended = !sended;

		if (notSended)
		{
			_notificationContext.AddNotification("Email", "Confirmation email not sent");
			_logger.LogError("[{ActionType}/AddAsync] User {Id} confirmation email not sent", ActionType,
				physicalPersonId);
		}

		return physicalPersonId;
	}
}