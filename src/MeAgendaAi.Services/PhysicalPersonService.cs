using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;


namespace MeAgendaAi.Services
{
    public class PhysicalPersonService : Service<PhysicalPerson>, IPhysicalPersonService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;

        public PhysicalPersonService(
            IUserService userService,
            IPhysicalPersonRepository physicalPersonRepository,
            NotificationContext notificationContext) : base(physicalPersonRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
        }

        public async Task<Guid> AddAsync(AddPhysicalPersonRequest request)
        {
            if (await _userService.HasUser(request.Email))
            {
                _notificationContext.AddNotification("Email", "Email já cadastrado");
                return Guid.Empty;
            }

            var physicalPerson = new PhysicalPerson(request.Email, request.Password, request.Name, request.Surname, request.CPF, request.RG);
            if (physicalPerson.Invalid)
            {
                _notificationContext.AddNotifications(physicalPerson.ValidationResult);
                return Guid.Empty;
            }

            return await AddAsync(physicalPerson);
        }
    }
}
