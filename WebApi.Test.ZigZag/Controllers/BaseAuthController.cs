using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZigZag.Test.Data;

namespace ZigZag.Test.Controllers;


[ApiController]
public abstract class BaseAuthController : BaseController
{
    private readonly AuthorisationMgr _authorisationMgr;
    protected Guid accessTokenUid { get;private set; }
    protected BaseAuthController(ApiConfig config,Db db,AuthorisationMgr authorisationMgr) : base(config,db) 
    {
        _authorisationMgr = authorisationMgr;
    }


    protected async sealed override Task<bool> LoadRequestData(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (await base.LoadRequestData(context, next))
        {
            accessTokenUid = AuthorisationMgr.GetAccessTokenUid(context.HttpContext.Request);

            return await Task.Run(() => _authorisationMgr.Authorize(base.applicationSessionUid, accessTokenUid));
        }
        else
        {
            return false;
        }
    }


}
