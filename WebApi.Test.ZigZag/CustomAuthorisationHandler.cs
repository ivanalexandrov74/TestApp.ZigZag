using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using MongoDB.Bson;

namespace ZigZag.Test
{
    public class CustomAuthorisationHandler : IAuthorizationHandler
    {
        private readonly AuthorisationMgr _authorisationMgr;
        public CustomAuthorisationHandler(AuthorisationMgr authorisationMgr)
        {
            _authorisationMgr = authorisationMgr;
        }
        public async ValueTask<AuthorizeResult> AuthorizeAsync(IMiddlewareContext context, AuthorizeDirective directive, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => AuthorizeResult.NotAllowed);
        }

        public async ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => AuthorizeResult.NotAllowed);
        }
    }
}
