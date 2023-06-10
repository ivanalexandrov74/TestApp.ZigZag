using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Authentication;

namespace ZigZag.Test.Data
{
    public class CustomHttpRequestInterceptor:DefaultHttpRequestInterceptor
    {
        private readonly AuthorisationMgr _authorisationMgr;
        public CustomHttpRequestInterceptor(AuthorisationMgr authorisationMgr) 
        {
            _authorisationMgr = authorisationMgr;
        }
        public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            if (!_authorisationMgr.Authorize(context.Request))
                return new ValueTask(Task.Run(async () => await context.ForbidAsync(), cancellationToken));

            return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        }
    }
}
