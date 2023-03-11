namespace YomLog.BlazorShared.Models;

public class TokenModel
{
    public string UserName { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset AccessTokenExpiration { get; set; }
}
