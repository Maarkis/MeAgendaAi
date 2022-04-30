using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAgendaAi.Application.Controllers
{
    public class CompanyController : StandardController
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        private const string ActionType = "CompanyController";

        public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger) =>
            (_companyService, _logger) = (companyService, logger);

        [HttpGet]
        [AllowAnonymous]
        [Route("Report")]
        public async Task<ActionResult> Report()
        {
            _logger.LogInformation("[{ActionType}/Report] Starting process to generate company report", ActionType);

            const string type = "csv";
            var nameArchive = $"Report_Company_{DateTime.Now.ToShortDateString()}.{type}";
            var report = await _companyService.ReportAsync();
            if (report == null)
                return NotFound(new BaseMessage("No companies found."));

            _logger.LogInformation("[{ActionType}/Report] Finalizing process to generate company report", ActionType);

            return File(report, "csv/text", nameArchive);
        }
    }
}