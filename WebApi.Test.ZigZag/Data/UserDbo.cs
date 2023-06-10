using MongoDB.Bson.Serialization.Attributes;

namespace ZigZag.Test.Data;

public class UserDbo
{
    [BsonId]
    public string userName { get; set; } = string.Empty;

    public string userPasswordSha { get; set; } = string.Empty;

}
