using System.Net;
using System.Net.Http.Json;
using YomLog.BlazorShared.Services.Auth;
using YomLog.BlazorShared.Services.Popup;
using YomLog.BlazorShared.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.Shared.Exceptions;

namespace YomLog.BlazorShared.Services;

public class HttpClientWrapper : BindBase
{
    private readonly HttpClient _httpClient;
    private readonly IPopupService _popupService;
    private readonly IAuthService _authService;

    private int _retryCount;
    private readonly int RetryLimit = 3;

    public ReactivePropertySlim<bool> IsOffline { get; }

    public HttpClientWrapper(HttpClient httpClient, IPopupService popupService, IAuthService authService)
    {
        _httpClient = httpClient;
        _popupService = popupService;
        _authService = authService;

        IsOffline = new ReactivePropertySlim<bool>().AddTo(Disposable);
    }

    public async Task<T> Get<T>(string uri)
        => (await HandleAsJsonAsync<T>(async () => await _httpClient.GetAsync(uri)))!;

    public async Task<TResult> Create<TResult, TCommand>
        (string uri, TCommand command)
         => (await HandleAsJsonAsync<TResult>(async () => await _httpClient.PostAsJsonAsync(uri, command)))!;

    public async Task<TResult> Put<TResult, TCommand>
        (string uri, TCommand command)
        => (await HandleAsJsonAsync<TResult>(async () => await _httpClient.PutAsJsonAsync(uri, command)))!;

    public async Task Delete(string uri)
        => await GetHttpResponseAsync(async () => await _httpClient.DeleteAsync(uri));

    public async Task<T?> HandleAsJsonAsync<T>
        (Func<Task<HttpResponseMessage>> callback)
    {
        var response = await GetHttpResponseAsync(callback);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<HttpResponseMessage> GetHttpResponseAsync(Func<Task<HttpResponseMessage>> callback)
    {
        HttpResponseMessage response = default!;
        try
        {
            response = await callback();
            response.EnsureSuccessStatusCode();
            IsOffline.Value = false;
            return response;
        }
        catch (Exception e)
        {
            IApiException exception = null!;

            if (e is HttpRequestException httpException)
            {
                switch (httpException.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        exception = await ModelErrorException.CreateFromHttpResponse(httpException, response);
                        break;
                    case HttpStatusCode.NotFound:
                        exception = await NotFoundException.CreateFromHttpResponse(httpException, response);
                        break;
                    case HttpStatusCode.Forbidden:
                        exception = await ForbiddenException.CreateFromHttpResponse(httpException, response);
                        break;
                    case HttpStatusCode.Unauthorized:
                        if (_retryCount++ > RetryLimit)
                        {
                            _retryCount = 0;
                            exception = await UnauthorizedException.CreateFromHttpResponse(httpException, response);
                            break;
                        }
                        await _authService.RefreshTokenAsync();
                        return await GetHttpResponseAsync(callback);
                    case null:
                        IsOffline.Value = true;
                        exception = await ApiException.CreateFromHttpResponse(httpException, response);
                        break;
                    default:
                        IsOffline.Value = true;
                        exception = await ApiException.CreateFromHttpResponse(httpException, response);
                        break;
                }
            }
            else
            {
                IsOffline.Value = true;
                httpException = new HttpRequestException(e.Message);
                exception = await ApiException.CreateFromHttpResponse(httpException, response);
            }

            if (exception.StatusCode != null && exception.StatusCode != HttpStatusCode.NotFound)
            {
                string message = exception.Response ?? exception.Message ?? e.Message;
                await _popupService.ShowPopup("API Error", message);
            }

            throw (Exception)exception;
        }
    }
}
