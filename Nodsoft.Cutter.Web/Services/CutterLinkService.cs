using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Nodsoft.Cutter.Web.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nodsoft.Cutter.Web.Services
{
	public class CutterLinkService
	{
		private IMongoCollection<CutterLink> CutterLinks { get; init; }
		public const int DefaultLinkLength = 8;
		private static readonly Random Random = new();


		public CutterLinkService(IConfiguration config)
		{
			IConfigurationSection mongoConfig = config.GetSection("MongoDatabase");
			MongoClient client = new(mongoConfig["ConnectionString"]);
			IMongoDatabase db = client.GetDatabase(mongoConfig["DatabaseName"]);

			CutterLinks = db.GetCollection<CutterLink>(mongoConfig["CutterLinksCollection"]);
		}

		public async Task<CutterLink> FetchCutter(string linkName) => await (await CutterLinks.FindAsync(x => x.Name == linkName)).FirstOrDefaultAsync();

		public async Task<CutterLink> CreateCutter(CutterLink cutter)
		{
			if (!await (await CutterLinks.FindAsync(x => x.Destination == cutter.Destination)).AnyAsync())
			{
				if (cutter.Name is null)
				{
					do
					{
						cutter.Name = GenerateCutterName();
					} 
					while (CutterLinks.Find(x => x.Name == cutter.Name).Any());
				}

				await CutterLinks.InsertOneAsync(cutter);
			}

			return await (await CutterLinks.FindAsync(x => x.Name == cutter.Name)).FirstOrDefaultAsync();
		}

		private static string GenerateCutterName()
		{
			byte[] bytes = new byte[DefaultLinkLength];
			Random.NextBytes(bytes);

			return Base62Converter.Decode(bytes);
		}
	}
}
