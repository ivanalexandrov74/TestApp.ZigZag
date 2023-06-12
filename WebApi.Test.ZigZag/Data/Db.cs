using MongoDB.Driver;

namespace ZigZag.Test.Data;

public class Db
{


    public Db(ApiConfig config)
    {
        var mongoClient = new MongoClient(config.mongoDbConnectionString);

        var mongoDatabase = mongoClient.GetDatabase("ZigZagDB");

        Users = mongoDatabase.GetCollection<UserDbo>("Users");

        Users.Indexes.CreateOne(new CreateIndexModel<UserDbo>(
            Builders<UserDbo>.IndexKeys.Ascending(user => user.UserName)
            , new CreateIndexOptions { Unique = true }));

        AccessTokens = mongoDatabase.GetCollection<AccessTokenDbo>("AccessTokens");

        _config = config;
    }

    private readonly ApiConfig _config;

    public readonly IMongoCollection<UserDbo> Users;

    public readonly IMongoCollection<AccessTokenDbo> AccessTokens;



    public void CreateDefaultUserIfMissing()
    {
        if (null == Users.Find(item => item.UserName.ToUpper() == _config.defaultUserName.ToUpper()).FirstOrDefault())
        {
            Users.InsertOne(new UserDbo
            {
                UserUid=Guid.NewGuid(),
                UserName = _config.defaultUserName,
                UserPasswordSha = _config.defaultUserPassword.ToSha256String()
            });
        }
    }
}
