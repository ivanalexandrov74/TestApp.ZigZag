

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.Runtime.Internal;
using Newtonsoft.Json;
using System.Net;

namespace ZigZag.Test.Data
{
    public class GraphQlClient
    {
        private readonly ApiConfig _config;
        private readonly Guid _applicationSessionUid;
        private readonly Guid _accessTokenUid;

        public GraphQlClient(ApiConfig config,Guid applicationSessionUid,Guid accessTokenUid)
        {
            _config = config;
            _applicationSessionUid = applicationSessionUid;
            _accessTokenUid = accessTokenUid;
        }


        public async Task<T> ExceuteQueryAsync<T>(string requestBody)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Accept", "application/graphql-response+json");
            httpClient.DefaultRequestHeaders.Add("applicationSessionUid", $"{_applicationSessionUid:N}");
            httpClient.DefaultRequestHeaders.Add("accessTokenUid", $"{_accessTokenUid:N}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_config.graphQlServiceUrl}"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var result = await httpClient.SendAsync(request);

            if (result == null)
                throw new WebException($"Did not receive response from GraphQL at address '{_config.graphQlServiceUrl}'!");
            else if (result.IsSuccessStatusCode)
            {
                var stringContent=await result.Content.ReadAsStringAsync();
#pragma warning disable CS8603 // Possible null reference return.
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(stringContent);
#pragma warning restore CS8603 // Possible null reference return.
            }

            else
            {
                throw new WebException($"{Convert.ToInt32(result?.StatusCode)}({result?.StatusCode}):  {result?.ReasonPhrase}");
            }

        }
    }
}
