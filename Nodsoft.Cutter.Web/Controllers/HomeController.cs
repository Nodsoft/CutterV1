using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nodsoft.Cutter.Web.Data.Models;
using Nodsoft.Cutter.Web.Models;
using Nodsoft.Cutter.Web.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nodsoft.Cutter.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> logger;
		private readonly CutterLinkService service;
		private static string externalCutterUri, externalLinksUri;

		public HomeController(ILogger<HomeController> logger, CutterLinkService cutterService, IConfiguration configuration)
		{
			this.logger = logger;
			service = cutterService;
			externalCutterUri ??= configuration["links:externalCutter"];
			externalLinksUri ??= configuration["links:externalLinks"];
		}
		public IActionResult Index() => View(new CutterLink());

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

		[HttpPost]
		public async Task<IActionResult> CreateCutterLink([Bind] CutterLink cutter)
		{
			cutter = await SubmitNewAsync(cutter);
			TempData["result"] = externalLinksUri + cutter.Name;

			return RedirectToAction(nameof(Index));
		}

		[HttpPost("cutter")]
		public async Task<IActionResult> CreateCutterLinkApi([FromBody] CutterLink cutter)
		{
			cutter = await SubmitNewAsync(cutter);
			return StatusCode(200, externalLinksUri + cutter.Name);
		}


		[HttpGet("go/{id}")]
		public async Task<IActionResult> RedirectToLink([FromRoute] string id)
		{
			CutterLink cutter = await service.FetchCutterAsync(id);

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
		public IActionResult RedirectToCutter() => RedirectPermanent(externalCutterUri);


		private async Task<CutterLink> SubmitNewAsync(CutterLink cutter)
		{
			cutter = cutter with
			{
				CreatedAt = DateTime.UtcNow,
				CreatedFromIp = HttpContext.Connection.RemoteIpAddress.ToString()
			};

			return await service.CreateCutterAsync(cutter);
		}
	}
}
