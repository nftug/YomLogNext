using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public abstract class ModelResultDTOBase<TModel, TModelDTO>
    : ModelDTOBase<TModel, TModelDTO>, IResultDTO
    where TModel : EntityBase<TModel>
    where TModelDTO : ModelResultDTOBase<TModel, TModelDTO>
{
    public long PK { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public UserReference CreatedBy { get; init; } = null!;
    public UserReference UpdatedBy { get; init; } = null!;

    protected ModelResultDTOBase(EntityBase<TModel> model) : base(model)
    {
        PK = model.PK;
        CreatedOn = model.DateTimeRecord.CreatedOn;
        UpdatedOn = model.DateTimeRecord.UpdatedOn;
        CreatedBy = model.UserRecord.CreatedBy;
        UpdatedBy = model.UserRecord.UpdatedBy;
    }

    public abstract ICommandDTO<TModel> ToCommandDTO();
}
