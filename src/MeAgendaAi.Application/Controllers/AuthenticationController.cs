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
        private readonly ICompanyService _companyService;

        public AuthenticationController(IPhysicalPersonService physicalPersonService, ICompanyService companyService)
        {
            _physicalPersonService = physicalPersonService;
            _companyService = companyService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddPhysicalPerson")]
        public async Task<ActionResult<ResponseBase<Guid>>> AddClient(AddPhysicalPersonRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _physicalPersonService.AddAsync(request);
            return Created("", new ResponseBase<Guid>(response, "Cadastrado com sucesso", true));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddCompany")]
        public async Task<ActionResult<ResponseBase<Guid>>> AddCompany(AddCompanyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _companyService.AddAsync(request);
            return Created("", new ResponseBase<Guid>(response, "Cadastrado com sucesso", true));
        }
    }
}
