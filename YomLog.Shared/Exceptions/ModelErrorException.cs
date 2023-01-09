using System.Text.Json;
using System.Text.Json.Serialization;

namespace YomLog.Shared.Exceptions;

public class ModelErrorException : ApiExceptionBase<ModelErrorException>
{
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>> Errors { get; init; } = new() { };

    public ModelErrorException(string field, string message)
    {
        Errors[field] = new List<string> { message };
    }

    public ModelErrorException(string message)
        : this("other", message)
    {
    }

    public ModelErrorException(IDictionary<string, List<string>> errors)
    {
        Errors = new(errors);
    }

    public ModelErrorException() { }

    public void Add(string field, string message)
    {
        if (!Errors.ContainsKey(field)) Errors[field] = new List<string> { };
        Errors[field].Add(message);
    }

    public static new async Task<ModelErrorException> CreateFromHttpResponse
        (HttpRequestException exception, HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var errorDetails = JsonSerializer.Deserialize<ErrorDetails>(body);
        var errors = errorDetails?.Errors ?? new Dictionary<string, List<string>>();

        return new()
        {
            Exception = exception,
            Response = body,
            Errors = new(errors)
        };
    }
}

public class ErrorDetails
{
    [JsonPropertyName("errors")]
    public IDictionary<string, List<string>>? Errors { get; set; }
}
