using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using ZigZag.Test.Data;
using ZigZag.Test.Dto;

namespace ZigZag.Test
{
    public sealed class AuthorisationMgr
    {
        private readonly ApiConfig _config;
        private readonly Db _db;
        private readonly int _susspiciousTokenUidCacheCapacity;
        private readonly int _accessTokenCacheCapacity;
        private readonly Queue<Guid> _susspiciousTokenUidCache;
        private readonly Queue<AccessTokenDbo> _accessTokenCache;


        public AuthorisationMgr(ApiConfig config, Db db)
        {
            _config = config;
            _db = db;
            _susspiciousTokenUidCacheCapacity = _config.susspiciousTokenUidCacheCapacity;
            _accessTokenCacheCapacity = _config.accessTokenCacheCapacity;
            _susspiciousTokenUidCache = new Queue<Guid>(_susspiciousTokenUidCacheCapacity);
            _accessTokenCache = new Queue< AccessTokenDbo> (_accessTokenCacheCapacity);
        }

        public bool Authorize(HttpRequest httpRequest) => Authorize(GetApplicationSessionUid(httpRequest), GetAccessTokenUid(httpRequest));

        public bool Authorize(Guid applicationSessionUid, Guid accessTokenUid)
        {
            if (applicationSessionUid == Guid.Empty || accessTokenUid == Guid.Empty)
                return false;
            else
            {
                var susspiciousRecord = _susspiciousTokenUidCache.FirstOrDefault(item => item == accessTokenUid);

                if (susspiciousRecord != Guid.Empty)
                    return false;

                var cachedRecord=_accessTokenCache.FirstOrDefault(item => item.AccessTokenUid == accessTokenUid);

                if (cachedRecord != null)
                {
                    if (cachedRecord.ValidToUtc > DateTime.UtcNow && cachedRecord.ApplicationSessionUid == applicationSessionUid)
                        return true;
                    else
                        return false;
                }
                else
                {
                    var dbRecord=_db.AccessTokens.Find(item => item.AccessTokenUid==accessTokenUid).FirstOrDefault();

                    if (dbRecord != null)
                    {
                        PushInCache(dbRecord);

                        if (dbRecord.ValidToUtc > DateTime.UtcNow && dbRecord.ApplicationSessionUid == applicationSessionUid)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        PushInSusspicious(accessTokenUid);

                        return false;
                    }
                }
            }
        }

        public void PushInCache(AccessTokenDbo dbRecord)
        {
            while (_accessTokenCache.Count >= _accessTokenCacheCapacity)
                _accessTokenCache.Dequeue();

            _accessTokenCache.Enqueue(dbRecord);
        }

        private void PushInSusspicious(Guid accessTokenUid)
        {
            while (_susspiciousTokenUidCache.Count>=_susspiciousTokenUidCacheCapacity)
                _susspiciousTokenUidCache.Dequeue();

            _susspiciousTokenUidCache.Enqueue(accessTokenUid);
        }

        public static Guid GetApplicationSessionUid(HttpRequest httpRequest) => GetHttpRequestHeaderUid(httpRequest, "applicationSessionUid");

        public static Guid GetAccessTokenUid(HttpRequest httpRequest) => GetHttpRequestHeaderUid(httpRequest, "accessTokenUid");

        private static Guid GetHttpRequestHeaderUid(HttpRequest httpRequest,string headerName)
        {
            if (Guid.TryParse(httpRequest.Headers[headerName], out Guid result))
                return result;
            else
                return Guid.Empty;
        }
    }
}
