namespace YomLog.BlazorShared.Services.Repository;

public interface IPreferenceRepositoryService
{
    Task<T?> GetAsync<T>(string key);
    Task SaveAsync<T>(string key, T value);
    Task RemoveAsync(string key);
}
