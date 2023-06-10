using ZigZag.Test.Dto;

namespace ZigZag.Test.Data
{
    public class Query
    {
        private readonly ExternalApiContext _context;
        public Query(ExternalApiContext context)
        {
            _context = context;
        }
        [UsePaging]
        public IQueryable<ExternalApiDto> ExternalApis(bool forceRefresh=false, string? category=null) => _context.ExternalApis(forceRefresh, category);
    }
}
