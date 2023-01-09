namespace YomLog.Shared.ValueObjects;

public class UserRecord : ValueObject<UserRecord>
{
    public UserReference CreatedBy { get; }
    public UserReference UpdatedBy { get; private set; }

    public UserRecord(UserReference createdBy, UserReference updatedBy)
    {
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
    }

    public UserRecord(UserReference createdBy) : this(createdBy, createdBy)
    {
    }

    public void ChangeUpdatedBy(UserReference updatedBy)
    {
        UpdatedBy = updatedBy;
    }

    protected override bool EqualsCore(UserRecord other)
        => CreatedBy == other.CreatedBy && UpdatedBy == other.UpdatedBy;
}

public class DateTimeRecord : ValueObject<DateTimeRecord>
{
    public DateTime CreatedOn { get; }
    public DateTime UpdatedOn { get; private set; }

    public DateTimeRecord(DateTime createdOn, DateTime updatedOn)
    {
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    public DateTimeRecord(DateTime createdOn) : this(createdOn, createdOn)
    {
    }

    public void ChangeUpdatedOn(DateTime updatedOn)
    {
        UpdatedOn = updatedOn;
    }

    protected override bool EqualsCore(DateTimeRecord other)
        => CreatedOn == other.CreatedOn && UpdatedOn == other.UpdatedOn;
}