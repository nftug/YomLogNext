using LiteDB.Async;
using YomLog.Infrastructure.Shared.DataModels;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Entities;

namespace YomLog.Infrastructure.Shared.LiteDB;

public class LiteDbCollection<TEntity, TDataModel> : IDisposable
    where TEntity : EntityBase<TEntity>
    where TDataModel : DataModelBase<TEntity, TDataModel>, new()
{
    private readonly ILiteDatabaseAsync _db;
    private readonly ILiteCollectionAsync<TDataModel> _collection;

    public ILiteCollectionAsync<TDataModel> Collection => _collection;

    private readonly AppConfig _appConfig;
    public string DbPath => Path.Combine(_appConfig.AppDataPath, "yomlog.db");

    private bool disposedValue;

    public LiteDbCollection(AppConfig appConfig)
    {
        _appConfig = appConfig;

        _db = new LiteDatabaseAsync(DbPath);
        string tableName = typeof(TEntity).Name.Pluralize();
        _collection = _db.GetCollection<TDataModel>(tableName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}