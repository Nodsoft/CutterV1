using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Nodsoft.Cutter.Web.Data.Models;

namespace Nodsoft.Cutter.Web.Data.Repositories
{
	public class CutterRepository
	{
		private IMongoCollection<CutterLink> CutterLinks { get; init; }

		public CutterRepository(IConfiguration configuration)
		{
			IConfigurationSection mongoConfig = configuration.GetSection("MongoDatabase");
			MongoClient client = new(mongoConfig["ConnectionString"]);
			IMongoDatabase db = client.GetDatabase(mongoConfig["DatabaseName"]);

			CutterLinks = db.GetCollection<CutterLink>(mongoConfig["CutterLinksCollection"]);
		}

		public async Task<CutterLink> GetbyIdAsync(string id) => await (await CutterLinks.FindAsync(x => x.Id == id)).FirstOrDefaultAsync();

		public async Task<CutterLink> GetByNameAsync(string name) => await (await CutterLinks.FindAsync(x => x.Name == name)).FirstOrDefaultAsync();

		public async Task<bool> LinkExists(string dest) => await (await CutterLinks.FindAsync(x => x.Destination == dest)).AnyAsync();

		public async Task InsertAsync(CutterLink link) => await CutterLinks.InsertOneAsync(link);
	}
}
