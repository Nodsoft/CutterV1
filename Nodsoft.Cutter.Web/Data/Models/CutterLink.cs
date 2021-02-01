using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nodsoft.Cutter.Web.Data.Models
{
	public record CutterLink
	{
		[BsonId, BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; init; }
		[BsonRequired, RegularExpression("/^[a-zA-Z0-9-_]+$/")]
		public string Name { get; set; }

		[Required, BsonRequired, Url]
		public string Destination { get; set; }

		[BsonRequired]
		public DateTime CreatedAt { get; init; }

		public string CreatedFromIp { get; set; }
	}
}
