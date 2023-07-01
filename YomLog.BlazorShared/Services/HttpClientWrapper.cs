using System.Net;
using System.Net.Http.Json;
using YomLog.BlazorShared.Services.Auth;
using YomLog.BlazorShared.Services.Popup;
using YomLog.BlazorShared.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.Shared.Exceptions;
using Dynamic.Json;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
public class HttpClientWrapper : BindableBase
{
    private readonly HttpClient _httpClient;
    private readonly IPopupService _popupService;
    private readonly IAuthService _authService;

    private int _retryCount;
    private readonly int RetryLimit = 3;

    private readonly ReactivePropertySlim<bool> _isOffline;
    public ReadOnlyReactivePropertySlim<bool> IsOffline { get; }

    public HttpClientWrapper(HttpClient httpClient, IPopupService popupService, IAuthService authService)
    {
        _httpClient = httpClient;
        _popupService = popupService;
        _authService = authService;

        _isOffline = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsOffline = _isOffline.ToReadOnlyReactivePropertySlim();
    }

    public Task<T> Get<T>(string uri)
        => HandleAsJsonAsync<T>(() => _httpClient.GetAsync(uri))!;

    public Task<dynamic> GetAsDJson(string uri)
        => HandleAsDJsonAsync(() => _httpClient.GetAsync(uri));

    public Task<TResult> Create<TResult, TCommand>(string uri, TCommand command)
        => HandleAsJsonAsync<TResult>(() => _httpClient.PostAsJsonAsync(uri, command))!;

    public Task<TResult> Create<TResult>(string uri, MultipartFormDataContent formData)
        => HandleAsJsonAsync<TResult>(() => _httpClient.PostAsync(uri, formData))!;

    public Task CreateWithoutResult<TCommand>(string uri, TCommand command)
        => GetHttpResponseAsync(() => _httpClient.PostAsJsonAsync(uri, command));

    public Task CreateWithoutResult(string uri, MultipartFormDataContent formData)
        => GetHttpResponseAsync(() => _httpClient.PostAsync(uri, formData));

    public Task<TResult> Put<TResult, TCommand>(string uri, TCommand command)
        => HandleAsJsonAsync<TResult>(() => _httpClient.PutAsJsonAsync(uri, command))!;

    public Task Delete(string uri)
        => GetHttpResponseAsync(() => _httpClient.DeleteAsync(uri));

    public async Task<T?> HandleAsJsonAsync<T>(Func<Task<HttpResponseMessage>> callback)
    {
        var response = await GetHttpResponseAsync(callback);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<dynamic> HandleAsDJsonAsync(Func<Task<HttpResponseMessage>> callback)
    {
        var response = await GetHttpResponseAsync(callback);
        var rawContent = await response.Content.ReadAsStringAsync();
        return DJson.Parse(rawContent);
    }

    public void RecoverConnectivityState() => _isOffline.Value = false;

    public async Task<HttpResponseMessage> GetHttpResponseAsync(Func<Task<HttpResponseMessage>> callback)
    {
        HttpResponseMessage response = default!;

        try
        {
            if (IsOffline.Value) throw new HttpRequestException();

            response = await callback();
            response.EnsureSuccessStatusCode();
            _isOffline.Value = false;
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
                        exception = await EntityValidationException.CreateFromHttpResponse(httpException, response);
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
                    case HttpStatusCode.InternalServerError:
                        exception = await ApiException.CreateFromHttpResponse(httpException, response);
                        break;
                    case null:
                        _isOffline.Value = true;
                        exception = await ApiException.CreateFromHttpResponse(httpException, response);
                        break;
                    default:
                        _isOffline.Value = true;
                        exception = await ApiException.CreateFromHttpResponse(httpException, response);
                        break;
                }
            }
            else
            {
                _isOffline.Value = true;
                httpException = new HttpRequestException(e.Message);
                exception = await ApiException.CreateFromHttpResponse(httpException, response);
            }

            if (!IsOffline.Value)
            {
                string message =
                    !string.IsNullOrEmpty(exception.Response) && exception.Response.Length <= 100
                    ? exception.Response
                    : exception.Message ?? e.Message;
                await _popupService.ShowPopup("APIエラー", message);
            }

            throw (Exception)exception;
        }
    }
}