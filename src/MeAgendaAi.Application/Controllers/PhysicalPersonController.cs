using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Byte;

namespace MeAgendaAi.Application.Controllers;

public class PhysicalPersonController : StandardController
{
	private readonly ILogger<PhysicalPersonController> _logger;
	private readonly IPhysicalPersonService _physicalPersonService;

	private const string ActionType = "PhysicalPersonController";

	public PhysicalPersonController(IPhysicalPersonService physicalPersonService,
		ILogger<PhysicalPersonController> logger) =>
		(_physicalPersonService, _logger) = (physicalPersonService, logger);
	
	
	[HttpGet]
	[AllowAnonymous]
	[Route("Report")]
	public async Task<ActionResult> Report()
	{
		_logger.LogInformation("[{ActionType}/Report] Starting process to generate physical person report", ActionType);

		const string type = "csv";
		var nameArchive = $"Report_PhysicalPerson_{DateTime.Now.ToShortDateString()}.{type}";

		var report = await _physicalPersonService.ReportAsync();
		if (report == null)
			return NotFound(new BaseMessage("No physical persons found."));
		
		_logger.LogInformation("[{ActionType}/Report] Finalizing process to generate physical person report", ActionType);

		return File(report, "csv/text", nameArchive);
	}

}