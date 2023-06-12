using MongoDB.Bson.Serialization.Attributes;

namespace ZigZag.Test.Data;

public class AccessTokenDbo
{
    [BsonId]
    public Guid AccessTokenUid { get; set; } = Guid.Empty;

    public Guid ApplicationSessionUid { get; set; } = Guid.Empty;

    public Guid UserUid { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime ValidToUtc { get; set; }
}
