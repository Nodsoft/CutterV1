using MongoDB.Driver;
using Nodsoft.Cutter.Web.Data.Models;

namespace Nodsoft.Cutter.Web.Services;

public sealed class CutterLinkService
{
	public const int DefaultLinkLength = 8;

	private IMongoCollection<CutterLink> CutterLinks { get; init; }
	private readonly ILogger<CutterLinkService> _logger;

	public CutterLinkService(IConfiguration config, ILogger<CutterLinkService> logger)
	{
		_logger = logger;

		IConfigurationSection mongoConfig = config.GetSection("MongoDatabase");
		MongoClient client = new(mongoConfig["ConnectionString"]);
		IMongoDatabase db = client.GetDatabase(mongoConfig["DatabaseName"]);

		CutterLinks = db.GetCollection<CutterLink>(mongoConfig["CutterLinksCollection"]);
	}

	public async Task<CutterLink> FetchCutterAsync(string linkName) => await (await CutterLinks.FindAsync(x => x.Name == linkName)).FirstOrDefaultAsync();

	public async Task<CutterLink> CreateCutterAsync(CutterLink cutter)
	{
		if (await (await CutterLinks.FindAsync(x => x.Destination == cutter.Destination)).FirstOrDefaultAsync() is { } existing)
		{
			return existing;
		}

		if (cutter.Name is null)
		{
			do
			{
				cutter.Name = GenerateCutterName();
			} while (await CutterLinks.Find(x => x.Name == cutter.Name).AnyAsync());
		}

		await CutterLinks.InsertOneAsync(cutter);

		_logger.LogInformation("Link '{Link}' was inserted with Name '{Name}' by User '{RemoteIP}'", cutter.Destination, cutter.Name ?? "N/A", cutter.CreatedFromIp);

		return await (await CutterLinks.FindAsync(x => x.Destination == cutter.Destination)).FirstOrDefaultAsync();
	}


	private static string GenerateCutterName() => Base62Generator.GenerateString(DefaultLinkLength);
}