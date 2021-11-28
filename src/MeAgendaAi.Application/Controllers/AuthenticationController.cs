using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Domains.Interfaces.Services;

namespace MeAgendaAi.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IPhysicalPersonService _physicalPersonService;
        public AuthenticationController(IPhysicalPersonService physicalPersonService)
        {
            _physicalPersonService = physicalPersonService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddPhysicalPerson")]
        public async Task<ActionResult<ResponseBase<Guid>>> AddClient(AddPhysicalPersonRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _physicalPersonService.AddAsync(request);
            return Ok(new ResponseBase<Guid>(response, "Cadastrado com sucesso", true));
        }
    }
}
