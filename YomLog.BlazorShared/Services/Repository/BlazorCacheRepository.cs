using Blazored.LocalStorage;
using YomLog.Shared.Extensions;

namespace YomLog.BlazorShared.Services.Repository;

[InjectAsScoped]
public class BlazorCacheRepository : ICacheRepositoryService
{
    private readonly ILocalStorageService _localStorage;

    public BlazorCacheRepository(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private static string GetPath(string directory, string fileName)
        => $"{directory}/{fileName}";

    public async Task<T?> GetAsync<T>(string directory, string fileName)
        => await _localStorage.GetItemAsync<T>(GetPath(directory, fileName));

    public async Task<T?> GetAsync<T>(string key)
        => await _localStorage.GetItemAsync<T>(key);

    public async Task RemoveAsync(string directory, string fileName)
        => await _localStorage.RemoveItemAsync(GetPath(directory, fileName));

    public async Task RemoveAsync(string key)
        => await _localStorage.RemoveItemAsync(key);

    public async Task RemoveDirectoryAsync(string directory)
    {
        var keys = await _localStorage.KeysAsync();
        await _localStorage.RemoveItemsAsync(keys.Where(x => x.StartsWith($"{directory}/")));
    }

    public async Task SaveAsync<T>(string directory, string fileName, T value)
        => await _localStorage.SetItemAsync(GetPath(directory, fileName), value);

    public async Task SaveAsync<T>(string key, T value)
        => await _localStorage.SetItemAsync(key, value);
}
