using YomLog.Shared.Entities;

namespace YomLog.Shared.ValueObjects;

public class UserRecord : ValueObject<UserRecord>
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

    protected override bool EqualsCore(UserRecord other)
        => CreatedBy == other.CreatedBy && UpdatedBy == other.UpdatedBy;
}

public class DateTimeRecord : ValueObject<DateTimeRecord>
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

    protected override bool EqualsCore(DateTimeRecord other)
        => CreatedOn == other.CreatedOn && UpdatedOn == other.UpdatedOn;
}