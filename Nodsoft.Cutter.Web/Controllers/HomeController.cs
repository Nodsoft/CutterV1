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

		public IActionResult Index() => View();

		public IActionResult Privacy() => View();

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

		[HttpPost("cutter")]
		public async Task<IActionResult> CreateCutterLink([FromBody] CutterLink cutter)
		{
			cutter = cutter with
			{
				CreatedAt = DateTime.UtcNow,
				CreatedFromIp = HttpContext.Connection.RemoteIpAddress.ToString() 
			};
			
			cutter = await service.CreateCutterAsync(cutter);


			return StatusCode(200, externalLinksUri + cutter.Name);
		}

		[HttpGet("model")]
		public IActionResult GetModel() => StatusCode(200, new CutterLink());


		[HttpGet("go/{id}")]
		public async Task<IActionResult> RedirectToCutterLink([FromRoute] string id)
		{
			if (id is null)
			{
				RedirectPermanent(externalCutterUri);
			}

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
	}
}
