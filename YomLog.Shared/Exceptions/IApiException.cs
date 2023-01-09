using System.Net;

namespace YomLog.Shared.Exceptions;

public interface IApiException
{
    HttpRequestException? Exception { get; init; }
    string? Response { get; init; }
    HttpStatusCode? StatusCode { get; }
    string? Message { get; }
}