namespace YomLog.BlazorShared.Services.Repository;

public interface ICacheRepositoryService
{
    Task<T?> GetAsync<T>(string key);
    Task SaveAsync<T>(string key, T value);
    Task RemoveAsync(string key);
    Task<T?> GetAsync<T>(string directory, string fileName);
    Task SaveAsync<T>(string directory, string fileName, T value);
    Task RemoveAsync(string directory, string fileName);
    Task RemoveDirectoryAsync(string directory);
}
