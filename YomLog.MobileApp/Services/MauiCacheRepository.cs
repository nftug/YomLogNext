using System.Text.Json;
using YomLog.BlazorShared.Services.Repository;

namespace YomLog.MobileApp.Services;

public class MauiCacheRepository : ICacheRepositoryService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        string fullPath = Path.Combine(AppDataPath, key);
        if (!File.Exists(fullPath)) return default;

        var json = await File.ReadAllTextAsync(fullPath);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SaveAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await File.WriteAllTextAsync(Path.Combine(AppDataPath, key), json);
    }

    public Task RemoveAsync(string key)
    {
        string fullPath = Path.Combine(AppDataPath, key);
        if (!File.Exists(fullPath)) return Task.FromResult(-1);

        File.Delete(fullPath);
        return Task.FromResult(0);
    }

    public async Task<T?> GetAsync<T>(string directory, string fileName)
    {
        string dirFullPath = Path.Combine(AppDataPath, directory);
        return await GetAsync<T>(Path.Combine(dirFullPath, fileName));
    }

    public async Task SaveAsync<T>(string directory, string fileName, T value)
    {
        string dirFullPath = Path.Combine(AppDataPath, directory);
        if (!Directory.Exists(dirFullPath))
            Directory.CreateDirectory(dirFullPath);

        await SaveAsync(Path.Combine(dirFullPath, fileName), value);
    }

    public async Task RemoveAsync(string directory, string fileName)
    {
        string dirFullPath = Path.Combine(AppDataPath, directory);
        await RemoveAsync(Path.Combine(dirFullPath, fileName));
    }

    public Task RemoveDirectoryAsync(string directory)
    {
        string fullPath = Path.Combine(AppDataPath, directory);
        if (!Directory.Exists(fullPath)) return Task.FromResult(-1);

        Directory.Delete(fullPath, true);
        return Task.FromResult(0);
    }

    private static string AppDataPath
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppInfo.Name);
}
