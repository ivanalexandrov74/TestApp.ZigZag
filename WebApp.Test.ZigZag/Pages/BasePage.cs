using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;
using ZigZag.Test.Dto;

namespace ZigZag.Test.Pages;

public abstract class BasePage : ComponentBase
{
    public delegate void WebApiResponseEventHandler();
    public delegate void WebApiResponseEventHandler<TResponseType>(TResponseType responseData);
    public delegate bool WebApiErrorEventHandler(HttpStatusCode? httpStatusCode, string? responseData, Exception exception);

    [Inject] internal AppConfig appConfig { get; set; } = null!;
    [Inject] public AppData appData { get; set; } = null!;
    [Inject] public NavigationManager navigationManager { get; set; } = null!;

    public bool loading { get; set; }


    protected void ShowPage(Type pageType)
    {
        appData.currentPage = pageType;

        InvokeAsync(appData.homePage.StateHasChanged);
    }

    protected async Task ShowErrorMessage(string errorMessage)
    {
        appData.errorMessage = errorMessage;
        
        await InvokeAsync(appData.applicationErrorPopUpObject.StateHasChanged);
    }

    #region "API Calls"

    protected async Task CallWebApiAsync<TResponseData>(
    string url,
    HttpMethodEnum httpMethod,
    object? requestData = null,
    WebApiResponseEventHandler<TResponseData>? onWebApiResponseReceived = null,
    WebApiErrorEventHandler? onWebApiErrorReceived = null) where TResponseData : BaseResponseDto
    {
        var loadingValue = loading;

        HttpResponseMessage? result = null;

        try
        {
            loading = true;

            if (loadingValue != true) await InvokeAsync(StateHasChanged);

            result = await DoRequestAndGetResponse(url, httpMethod, requestData);

            if (result == null)
                throw new Exception($"Did not receive response from '{httpMethod}' Web API request at address '{url}'!");
            else if (result.IsSuccessStatusCode)
            {
                var loginResponseDto = await result.Content.ReadFromJsonAsync<TResponseData>();

                if (loginResponseDto == null)
                    throw new Exception($"An empty response received from '{httpMethod}' Web API request at address '{url}'!");
                else if (!loginResponseDto.isSuccess)
                    ShowWebApiFailedResult(loginResponseDto);
                else if (onWebApiResponseReceived != null)
                    onWebApiResponseReceived(loginResponseDto);
            }
            else
            {
                throw new WebException($"{Convert.ToInt32(result?.StatusCode)}({result?.StatusCode}):  {result?.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            bool susspendErrorMessage = false;
            if (onWebApiErrorReceived != null)
                susspendErrorMessage = onWebApiErrorReceived(result?.StatusCode, result?.ReasonPhrase, ex);
            else

            if (!susspendErrorMessage)
                ShowErrorMessage(
                    $"Web API call error - {GetWebApiErrorText(result?.StatusCode, result?.ReasonPhrase,ex)}!");
        }
        finally
        {
            if (!loadingValue)
            {
                loading = false;

                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async void ShowWebApiFailedResult<TResponseData>(TResponseData loginResponseDto) where TResponseData : BaseResponseDto
    {
        var props = loginResponseDto.GetType().GetProperties();

        var resultProp = loginResponseDto.GetType().GetProperties().FirstOrDefault(item => item.Name == nameof(BaseResponseDto<Enum>.result));

        if (resultProp == null)
        {
            await ShowErrorMessage($"Invalid web API response type. '{loginResponseDto.GetType().FullName}' has no deffined property 'result' or property type is not 'Enum' type!");

            return;
        }

        var enumValueMemberInfo = resultProp.PropertyType
            .GetMember($"{resultProp.GetValue(loginResponseDto)}")?
            .FirstOrDefault(m => m.DeclaringType == resultProp.PropertyType);

        if (enumValueMemberInfo == null)
        {
            await ShowErrorMessage($"Invalid web API response type. '{loginResponseDto.GetType().FullName}' has no deffined property 'result' or property type is not 'Enum' type!");

            return;
        }

        var valueAttribute = enumValueMemberInfo.GetCustomAttributes(typeof(EnumResultDescriptionAttribute), false).FirstOrDefault();

        var message
            = (valueAttribute != null)
            ? ((EnumResultDescriptionAttribute)valueAttribute).Description
            : $"{enumValueMemberInfo.Name}".Replace("_", " ");

        await ShowErrorMessage(message);
    }

    protected async Task CallWebApiAsync(
        string url,
        HttpMethodEnum httpMethod,
        object? requestData = null,
        WebApiResponseEventHandler? onWebApiResponseReceived = null,
        WebApiErrorEventHandler? onWebApiErrorReceived = null)
    {
        var loadingValue = loading;

        HttpResponseMessage? result = null;

        try
        {
            loading = true;

            if (loadingValue != true) await InvokeAsync(StateHasChanged);

            result = await DoRequestAndGetResponse(url, httpMethod, requestData);

            if (result == null)
                throw new Exception($"Did not receive response from '{httpMethod}' Web API request at address '{url}'!");
            else if (result.IsSuccessStatusCode)
            {
                if (onWebApiResponseReceived != null)
                {
                    onWebApiResponseReceived();
                }
            }
            else
            {
                throw new WebException($"{Convert.ToInt32(result?.StatusCode)}({result?.StatusCode}):  {result?.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            bool susspendErrorMessage = false;

            if (onWebApiErrorReceived != null)
                susspendErrorMessage = onWebApiErrorReceived(result?.StatusCode, result?.ReasonPhrase, ex);

            if (!susspendErrorMessage)
                await ShowErrorMessage(
                    $"Web API call error - {GetWebApiErrorText(result?.StatusCode, result?.ReasonPhrase, ex)}!");
        }
        finally
        {
            if (!loadingValue)
            {
                loading = false;

                await InvokeAsync(StateHasChanged);
            }
        }
    }

    protected virtual string GetWebApiErrorText(HttpStatusCode? statusCode, string? reasonPhrase,Exception? exception)
    {
        if (statusCode == null && string.IsNullOrWhiteSpace(reasonPhrase))
            return exception.GetExcetionFullText();
        else if (statusCode == null && !string.IsNullOrWhiteSpace(reasonPhrase))
            return $"?(?): {reasonPhrase}";
        else if (statusCode != null && string.IsNullOrWhiteSpace(reasonPhrase))
            return $"{Convert.ToInt32(statusCode)}({statusCode}): ";
        else
            return $"{Convert.ToInt32(statusCode)}({statusCode}): {reasonPhrase}";
    }


    private async Task<HttpResponseMessage?> DoRequestAndGetResponse(string url, HttpMethodEnum httpMethod, object? requestData)
    {
        var httpClient= new HttpClient { BaseAddress = new Uri(appConfig.webApiUrl) };

        httpClient.DefaultRequestHeaders.Add("applicationSessionUid", appData.applicationSessionUId.ToString("N"));

        httpClient.DefaultRequestHeaders.Add("accessTokenUid", appData.accessTokenUid.ToString("N"));

        switch (httpMethod)
        {
            case HttpMethodEnum.Get:
                if (requestData == null)
                    return await httpClient.GetAsync($"{httpClient.BaseAddress}{url}");
                else
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{httpClient.BaseAddress}{url}"),
                        Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                    };

                    return await httpClient.SendAsync(request);
                }

            case HttpMethodEnum.Put:
                if (requestData == null)
                    throw new ArgumentNullException($"Attempt to call WebApi '{httpMethod}' request at address '{url}' with 'requestData=NULL' parameter!");
                else
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri($"{httpClient.BaseAddress}{url}"),
                        Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                    };

                    return await httpClient.SendAsync(request);
                }
            case HttpMethodEnum.Delete:
                if (requestData == null)
                    throw new ArgumentNullException($"Attempt to call WebApi '{httpMethod}' request at address '{url}' with 'requestData=NULL' parameter!");
                else
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri($"{httpClient.BaseAddress}{url}"),
                        Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                    };

                    return await httpClient.SendAsync(request);
                }
            case HttpMethodEnum.Post:
                if (requestData == null)
                    throw new ArgumentNullException($"Attempt to call WebApi '{httpMethod}' request at address '{url}' with 'requestData=NULL' parameter!");
                else
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{httpClient.BaseAddress}{url}"),
                        Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                    };

                    return await httpClient.SendAsync(request);
                }

            case HttpMethodEnum.Patch:
                if (requestData == null)
                    throw new ArgumentNullException($"Attempt to call WebApi '{httpMethod}' request at address '{url}' with 'requestData=NULL' parameter!");
                else
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Patch,
                        RequestUri = new Uri($"{httpClient.BaseAddress}{url}"),
                        Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                    };

                    return await httpClient.SendAsync(request);
                }

            default:
                throw new ArgumentOutOfRangeException($"Unsupported '{nameof(httpMethod)}' parameter value '{httpMethod}'!");
        };
    }

    protected async Task<TResponse?> GetResponseDto<TResponse>(HttpResponseMessage result1) where TResponse : BaseResponseDto
    {
        try
        {
            return await result1.Content.ReadFromJsonAsync<TResponse>();
        }
        catch
        {
            return null;
        }
    }

    #endregion

}

public abstract class BasePage<TState> : BasePage where TState : class, new()
{

    public TState state { get; set; } = new TState();
}
