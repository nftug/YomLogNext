using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public abstract class EntityDetailsDTOBase<TEntity, TEntityDTO> : EntityDTOBase<TEntity, TEntityDTO>, IDetailsDTO
    where TEntity : EntityBase<TEntity>
    where TEntityDTO : EntityDetailsDTOBase<TEntity, TEntityDTO>
{
    public long PK { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }
    public EntityReferenceWithName<User> CreatedBy { get; init; } = null!;
    public EntityReferenceWithName<User>? UpdatedBy { get; init; }

    protected EntityDetailsDTOBase(EntityBase<TEntity> model) : base(model)
    {
        PK = model.PK;
        CreatedOn = model.DateTimeRecord.CreatedOn;
        UpdatedOn = model.DateTimeRecord.UpdatedOn;
        CreatedBy = model.UserRecord.CreatedBy;
        UpdatedBy = model.UserRecord.UpdatedBy;
    }

    public abstract ICommandDTO<TEntity> ToCommandDTO();
}
