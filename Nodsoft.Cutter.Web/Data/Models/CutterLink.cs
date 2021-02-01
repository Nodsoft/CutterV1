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

		public string Name { get; set; }

		[Required, Url]
		public string Destination { get; set; }

		public DateTime CreatedAt { get; init; }
		public string CreatedFromIp { get; set; }
	}
}
