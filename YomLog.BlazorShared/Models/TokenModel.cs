namespace YomLog.BlazorShared.Models;

public record TokenModel(
    string UserName,
    string UserId,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiration
);
