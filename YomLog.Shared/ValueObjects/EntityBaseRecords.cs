using YomLog.Shared.Entities;

namespace YomLog.Shared.ValueObjects;

public record UserRecord
{
    public EntityReferenceWithName<User> CreatedBy { get; }
    public EntityReferenceWithName<User>? UpdatedBy { get; private set; }

    public UserRecord(EntityReferenceWithName<User> createdBy, EntityReferenceWithName<User>? updatedBy)
    {
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
    }

    public UserRecord(EntityReferenceWithName<User> createdBy)
    {
        CreatedBy = createdBy;
    }

    public void ChangeUpdatedBy(EntityReferenceWithName<User> updatedBy)
    {
        UpdatedBy = updatedBy;
    }
}

public record DateTimeRecord
{
    public DateTime CreatedOn { get; }
    public DateTime? UpdatedOn { get; private set; }

    public DateTimeRecord(DateTime createdOn, DateTime? updatedOn)
    {
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    public DateTimeRecord(DateTime createdOn)
    {
        CreatedOn = createdOn;
    }

    public void ChangeUpdatedOn(DateTime updatedOn)
    {
        UpdatedOn = updatedOn;
    }
}