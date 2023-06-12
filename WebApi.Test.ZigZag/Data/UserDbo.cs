using MongoDB.Bson.Serialization.Attributes;

namespace ZigZag.Test.Data;

public class UserDbo
{
    [BsonId]
    public Guid UserUid { get; set; }
    public string UserName { get; set; } = string.Empty;

    public string UserPasswordSha { get; set; } = string.Empty;

}
