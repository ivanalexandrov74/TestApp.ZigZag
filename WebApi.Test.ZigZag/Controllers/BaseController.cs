using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using ZigZag.Test.Data;
using ZigZag.Test.Dto;

namespace ZigZag.Test.Controllers;

public abstract class BaseController : Controller
{

    protected readonly ApiConfig config;

    protected readonly Db db;

    protected Guid applicationSessionUid { get;private set; }


    protected BaseController(ApiConfig config,Db db)
    {
        this.config = config;
        this.db = db;
    }

    public sealed override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            if (await LoadRequestData(context,next))
                await base.OnActionExecutionAsync(context, next);
        }
        catch(Exception ex)
        {
            context.Result = ErrorResult(HttpStatusCode.InternalServerError, ex.GetExcetionFullText());
        }

    }


    protected virtual Task<bool> LoadRequestData(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return Task.Run(() => 
        {
            applicationSessionUid = GetApplicationSessionUidFromHeaders(context.HttpContext.Request.Headers);

            if (applicationSessionUid == Guid.Empty)
            {
                context.Result = BadRequest("'applicationSessionUid' request header is missing!");

                return false;
            }
            else
                return true;
        });
       
    }

    private Guid GetApplicationSessionUidFromHeaders(IHeaderDictionary headers)
    {
        Guid result;

        var header = headers["applicationSessionUid"];

        if (header == Microsoft.Extensions.Primitives.StringValues.Empty)
            return Guid.Empty;
        else if (Guid.TryParse(header, out result))
            return result;
        else
            return Guid.Empty;
    }




    private ActionResult ErrorResult(HttpStatusCode httpStatusCode, string description) => StatusCode((int)httpStatusCode, description);

    protected ActionResult<TResult> FailedResult<TResultEnum, TResult>(TResultEnum failedResult) where TResultEnum : Enum where TResult : BaseResponseDto<TResultEnum>, new()
    {
        var result = new TResult
        {
            result = failedResult,
        };

        return base.Ok(result);
    }

    protected ActionResult<TResult> FailedResult<TResultEnum, TResult>(TResultEnum failedResult, long logId) where TResultEnum : Enum where TResult : BaseResponseDto<TResultEnum>, new()
    {
        var result = new TResult
        {
            result = failedResult,
            logId = logId,
        };

        return base.Ok(result);
    }




    private new OkResult Ok() => base.Ok();

    private new OkObjectResult Ok([ActionResultObjectValue] object? value)
    => new OkObjectResult(value);


}
