using ZigZag.Test.Dto;

namespace ZigZag.Test.Data
{
    public class ExternalApiContext

    {
        private readonly ApiConfig _config;

        public ExternalApiContext(ApiConfig config)
        {
            _config = config;
        }

        public DateTime? dataPickedOn { get; private set; }

        public IQueryable<ExternalApiDto> ExternalApis(bool forceRequest, string? category = null)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            if (null == __externalApis || forceRequest || dataPickedOn.Value.Add(_config.externalApiDataLifetime) < DateTime.UtcNow)
            {
                var externalApiResult = new ExternalApiCallMgr<ExternalApiResponseDto>(_config.externalApiUrl).GetAsync().Result;

                __externalApis = externalApiResult.entries.AsQueryable();

                dataPickedOn = DateTime.UtcNow;
            }
#pragma warning restore CS8629 // Nullable value type may be null.

            if (string.IsNullOrWhiteSpace(category))
                return __externalApis;
            else
                return __externalApis.Where(item => item.Category == category);
        }

        private IQueryable<ExternalApiDto>? __externalApis;
    }
}
