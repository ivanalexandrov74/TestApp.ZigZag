using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZigZag.Test.Data;
using ZigZag.Test.Dto;
using HotChocolate.Language;


namespace ZigZag.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalApisController : BaseAuthController
    {
        private readonly ExternalApiContext _context;
        public ExternalApisController(ApiConfig config, Db db, AuthorisationMgr authorisationMgr, ExternalApiContext context) : base(config, db, authorisationMgr)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<ExternalApisResponseDto>> Get([FromBody] ExternalApisRequestDto requestData)
        {
            try
            {
                string requestBody;

                if (!string.IsNullOrEmpty(requestData.after))
                    requestBody = @"
{
  ""query"": ""{\r\n    externalApis( forceRefresh: false, category: \""{{CATEGORY}}\"", after: \""{{AFTER}}\"") {\r\n      nodes {\r\n        api\r\n        description\r\n        auth\r\n        cors\r\n        link\r\n      }\r\n      pageInfo {\r\n        hasNextPage\r\n        hasPreviousPage\r\n        startCursor\r\n        endCursor\r\n      }\r\n    }\r\n}""
}"
                    .Replace("{{CATEGORY}}", requestData.category)
                    .Replace("{{AFTER}}", requestData.after);
                else
                    requestBody = @"
{
  ""query"": ""{\r\n    externalApis( forceRefresh: false, category: \""{{CATEGORY}}\"") {\r\n      nodes {\r\n        api\r\n        description\r\n        auth\r\n        cors\r\n        link\r\n      }\r\n      pageInfo {\r\n        hasNextPage\r\n        hasPreviousPage\r\n        startCursor\r\n        endCursor\r\n      }\r\n    }\r\n}""
}
                    ".Replace("{{CATEGORY}}", requestData.category);

                var graphQlHttpClient = new GraphQlClient(config, applicationSessionUid, accessTokenUid);

                var result = await graphQlHttpClient.ExceuteQueryAsync<GraphQlExternalApiResponseDto>(requestBody);

                return new ExternalApisResponseDto
                {
                    externalApis = result.data.externalApis.nodes,
                    pageInfo = result.data.externalApis.pageInfo,
                    result = ExternalApisResultEnum.Success
                };
            }
            catch 
            {
                //It is good to log error details here because this error handling hide the error outside...

                return new ExternalApisResponseDto
                {
                    result = ExternalApisResultEnum.External_Api_Error
                };
            }
        }
    }
}
