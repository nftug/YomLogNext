using System.Net;

namespace YomLog.Shared.Exceptions;

public abstract class ApiExceptionBase<TSelf> : Exception, IApiException
    where TSelf : ApiExceptionBase<TSelf>, new()
{
    public HttpRequestException? Exception { get; init; }
    public string? Response { get; init; }

    public HttpStatusCode? StatusCode => Exception?.StatusCode;
    public new string? Message => Exception?.Message;

    public static async Task<TSelf> CreateFromHttpResponse
        (HttpRequestException exception, HttpResponseMessage? response)
        => new TSelf
        {
            Exception = exception,
            Response = response != null ? await response.Content.ReadAsStringAsync() : null
        };
}
