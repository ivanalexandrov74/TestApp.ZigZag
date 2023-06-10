using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZigZag.Test.Data;
using ZigZag.Test.Dto;

namespace ZigZag.Test.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : BaseAuthController
{
    private readonly ExternalApiContext _externalApiContext;
    public CategoriesController(ApiConfig config, Db db,AuthorisationMgr authorisationMgr, ExternalApiContext context) : base(config, db, authorisationMgr)
    {
        _externalApiContext = context;
    }


    [HttpGet]
    public ActionResult<CategoriesResponseDto> Get()
    {
        try
        {
            return new CategoriesResponseDto
            {
                categories = _externalApiContext.ExternalApis(false).Select(item => item.Category).Distinct().ToList(),
                result = CategoriesResultEnum.Success
            };
        }
        catch 
        {
            //It is good to log error details here because this error handling hide the error outside...

            return new CategoriesResponseDto
            {
                result = CategoriesResultEnum.External_Api_Error
            };
        }
    }
}
