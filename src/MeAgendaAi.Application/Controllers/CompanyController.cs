using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAgendaAi.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Report")]
        public async Task<ActionResult> Report()
        {
            var type = "csv";
            var nameArchive = $"Report_Company_{DateTime.Now.ToShortDateString()}.{type}";
            var report = await _companyService.ReportAsync();
            if (report == null)
                return NotFound(new BaseMessage("Nenhuma companhia encotrada."));

            return File(report, "csv/text", nameArchive);
        }

    }
}
