using Blazored.LocalStorage;
using YomLog.Shared.Extensions;

namespace YomLog.BlazorShared.Services.Repository;

[InjectAsScoped]
public class BlazorPreferenceRepository : IPreferenceRepositoryService
{
    private readonly ILocalStorageService _localStorage;

    public BlazorPreferenceRepository(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<T?> GetAsync<T>(string key)
        => await _localStorage.GetItemAsync<T>(key);

    public async Task RemoveAsync(string key)
        => await _localStorage.RemoveItemAsync(key);

    public async Task SaveAsync<T>(string key, T value)
        => await _localStorage.SetItemAsync(key, value);
}
