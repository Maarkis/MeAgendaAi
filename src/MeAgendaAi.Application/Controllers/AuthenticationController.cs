using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAgendaAi.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IPhysicalPersonService _physicalPersonService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<AuthenticationController> _logger;

        private const string ActionType = "AuthenticationController";

        public AuthenticationController(
            IPhysicalPersonService physicalPersonService,
            ICompanyService companyService,
            ILogger<AuthenticationController> logger) =>
            (_physicalPersonService, _companyService, _logger) = (physicalPersonService, companyService, logger);

        [HttpPost]
        [AllowAnonymous]
        [Route("AddPhysicalPerson")]
        public async Task<ActionResult<SuccessMessage<Guid>>> AddClient(AddPhysicalPersonRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/AddClient] Starting registration physical person process.", ActionType);

            var response = await _physicalPersonService.AddAsync(request);

            _logger.LogInformation("[{ActionType}/AddClient] Completing the physical person registration process.", ActionType);

            return Created("", new SuccessMessage<Guid>(response, "Cadastrado com sucesso"));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddCompany")]
        public async Task<ActionResult<SuccessMessage<Guid>>> AddCompany(AddCompanyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/AddCompany] Starting registration company process.", ActionType);

            var response = await _companyService.AddAsync(request);

            _logger.LogInformation("[{ActionType}/AddCompany] Completing the company registration process.", ActionType);

            return Created("", new SuccessMessage<Guid>(response, "Cadastrado com sucesso"));
        }
    }
}