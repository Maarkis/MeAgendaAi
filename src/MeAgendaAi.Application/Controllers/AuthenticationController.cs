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
        private readonly IUserService _userService;
        private readonly IPhysicalPersonService _physicalPersonService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<AuthenticationController> _logger;
        private const string ActionType = "AuthenticationController";

        public AuthenticationController(
            IUserService userService,
            IPhysicalPersonService physicalPersonService,
            ICompanyService companyService,
            ILogger<AuthenticationController> logger) =>
            (_userService, _physicalPersonService, _companyService, _logger) =
            (userService, physicalPersonService, companyService, logger);

        /// <summary>
        ///     Method for user authentication in the system.
        /// </summary>
        /// <param name="request">Object for authentication by email and password.</param>
        /// <returns>Return authentication object successfully.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<ActionResult<SuccessMessage<AuthenticateResponse>>> Authenticate(AuthenticateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/Authenticate] Starting user {request.Email} authentication process.", ActionType, request.Email);

            var result = await _userService.AuthenticateAsync(request.Email, request.Password);

            _logger.LogInformation("[{ActionType}/Authenticate] Completing the authentication process.", ActionType);

            return Ok(new SuccessMessage<AuthenticateResponse>(result!, "Successfully authenticated"));
        }

        /// <summary>
        ///     Method for user authentication in the system.
        /// </summary>
        /// <param name="refreshToken">Object for authentication by refresh token.</param>
        /// <returns>Return authentication object successfully.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RefreshToken")]
        public async Task<ActionResult<SuccessMessage<AuthenticateResponse>>> RefreshToken(string refreshToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/RefreshToken] Starting refresh token authentication process.", ActionType);

            var result = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

            _logger.LogInformation("[{ActionType}/RefreshToken] Completing the authentication process.", ActionType);

            return Ok(new SuccessMessage<AuthenticateResponse>(result!, "Successfully authenticated"));
        }

        /// <summary>
        ///     Method for add a physical person.
        /// </summary>
        /// <param name="request">Object for add physical person</param>
        /// <returns>Return added physical id</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("AddPhysicalPerson")]
        public async Task<ActionResult<SuccessMessage<Guid>>> AddClient(AddPhysicalPersonRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/AddClient] Starting registration physical person process.", ActionType);

            var result = await _physicalPersonService.AddAsync(request);

            _logger.LogInformation("[{ActionType}/AddClient] Completing the physical person registration process.", ActionType);

            return Created("", new SuccessMessage<Guid>(result, "Cadastrado com sucesso"));
        }

        /// <summary>
        ///     Method for add a company.
        /// </summary>
        /// <param name="request">Object for add physical person</param>
        /// <returns>Return added company id</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("AddCompany")]
        public async Task<ActionResult<SuccessMessage<Guid>>> AddCompany(AddCompanyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("[{ActionType}/AddCompany] Starting registration company process.", ActionType);

            var result = await _companyService.AddAsync(request);

            _logger.LogInformation("[{ActionType}/AddCompany] Completing the company registration process.", ActionType);

            return Created("", new SuccessMessage<Guid>(result, "Cadastrado com sucesso"));
        }
    }
}