using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nodsoft.Cutter.Web.Data.Models;

public sealed record CutterLink
{
	[BsonId, BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; init; } = string.Empty;
	[BsonRequired, RegularExpression(@"^[a-zA-Z\d-_]+$")]
	public string? Name { get; set; }

	[Required, BsonRequired, Url]
	public string Destination { get; set; } = string.Empty;

	[BsonRequired]
	public DateTime CreatedAt { get; init; }

	public string? CreatedFromIp { get; set; }
}