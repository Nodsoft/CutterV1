using Microsoft.AspNetCore.Mvc;
using Nodsoft.Cutter.Web.Data.Models;
using Nodsoft.Cutter.Web.Models;
using Nodsoft.Cutter.Web.Services;
using System.Diagnostics;

namespace Nodsoft.Cutter.Web.Controllers;

public sealed class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly CutterLinkService _service;
	private static string? _externalCutterUri, _externalLinksUri;

	public HomeController(ILogger<HomeController> logger, CutterLinkService cutterService, IConfiguration configuration)
	{
		_logger = logger;
		_service = cutterService;
		_externalCutterUri ??= configuration["links:externalCutter"];
		_externalLinksUri ??= configuration["links:externalLinks"];
	}
	public IActionResult Index() => View(new CutterLink());

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

	[HttpPost]
	public async Task<IActionResult> CreateCutterLink([Bind] CutterLink cutter)
	{
		cutter = await SubmitNewAsync(cutter);
		TempData["result"] = _externalLinksUri + cutter.Name;

		return RedirectToAction(nameof(Index));
	}

	[HttpPost("cutter")]
	public async Task<IActionResult> CreateCutterLinkApi([FromBody] CutterLink cutter)
	{
		cutter = await SubmitNewAsync(cutter);
		return StatusCode(200, _externalLinksUri + cutter.Name);
	}


	[HttpGet("go/{id}")]
	public async Task<IActionResult> RedirectToLink([FromRoute] string id)
	{
		CutterLink cutter = await _service.FetchCutterAsync(id);

		if (cutter is null)
		{
			return StatusCode(404);
		}
		else
		{
			return RedirectPermanent(cutter.Destination);
		}
	}

	[HttpGet("go")]
	public IActionResult RedirectToCutter() => RedirectPermanent(_externalCutterUri);


	private async Task<CutterLink> SubmitNewAsync(CutterLink cutter)
	{
		string remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();
		cutter = cutter with
		{
			CreatedAt = DateTime.UtcNow,
			CreatedFromIp = remoteIp
		};

		return await _service.CreateCutterAsync(cutter);
	}
}