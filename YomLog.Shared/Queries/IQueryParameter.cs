using YomLog.Shared.Entities;

namespace YomLog.Shared.Queries;

public interface IQueryParameter<TEntity>
    where TEntity : EntityBase<TEntity>
{
    int StartIndex { get; set; }
    int Limit { get; set; }
    string? Sort { get; set; }
    string? CreatedByName { get; set; }
    Guid? CreatedById { get; set; }
    string? Q { get; set; }
}
