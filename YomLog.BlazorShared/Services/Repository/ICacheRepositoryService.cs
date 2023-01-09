namespace YomLog.BlazorShared.Services.Repository;

public interface ICacheRepositoryService : IPreferenceRepositoryService
{
    Task<T?> GetAsync<T>(string directory, string fileName);
    Task SaveAsync<T>(string directory, string fileName, T value);
    Task RemoveAsync(string directory, string fileName);
    Task RemoveDirectoryAsync(string directory);
}
