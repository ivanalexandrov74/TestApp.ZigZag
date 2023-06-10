using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using GreenDonut;
using System.Net;
using System;

namespace ZigZag.Test.Data;

public class ExternalApiCallMgr<TResp>
{
    private readonly string _url;
    public ExternalApiCallMgr(string url)
    {
        _url = url;
    }


    public async Task<TResp> GetAsync()
    {
        var httpClient = new HttpClient();

        var rawResponse = await httpClient.GetAsync(_url);

        if (rawResponse == null)
            throw new Exception($"Did not receive response from 'GET' Web API request at address '{_url}'!");
        else if (rawResponse.IsSuccessStatusCode)
        {
            var result = await rawResponse.Content.ReadFromJsonAsync<TResp>();

            if (result == null)
                throw new Exception($"An empty response received from 'GET' Web API request at address '{_url}'!");
            else
                return result;
        }
        else
        {
            throw new WebException($"{Convert.ToInt32(rawResponse?.StatusCode)}({rawResponse?.StatusCode}):  {rawResponse?.ReasonPhrase}");
        }
    }
}
