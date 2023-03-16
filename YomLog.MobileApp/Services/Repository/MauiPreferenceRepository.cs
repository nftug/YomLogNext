using System.Text.Json;
using YomLog.BlazorShared.Services.Repository;
using YomLog.Shared.Attributes;

namespace YomLog.MobileApp.Services.Repository;

[InjectAsScoped]
public class MauiPreferenceRepository : IPreferenceRepositoryService
{
    public Task<T?> GetAsync<T>(string key)
    {
        var json = Preferences.Get(key, null);
        var result = json != null ? JsonSerializer.Deserialize<T>(json) : default;

        return Task.FromResult(result);
    }

    public Task SaveAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        Preferences.Set(key, json);

        return Task.FromResult(0);
    }

    public Task RemoveAsync(string key)
    {
        Preferences.Remove(key);
        return Task.FromResult(0);
    }
}