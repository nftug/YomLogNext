using YomLog.Shared.Exceptions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.Entities;

public abstract class EntityBase<T> : ValueObject<T>
    where T : EntityBase<T>
{
    public ModelReference<T> Reference { get; private set; } = null!;
    public DateTimeRecord DateTimeRecord { get; private set; } = null!;
    public UserRecord UserRecord { get; private set; } = null!;

    public long PK => Reference.PK;
    public Guid Id => Reference.Id;

    // データモデルからの変換用
    protected EntityBase(ModelReference<T> reference, DateTimeRecord dateTimeRecord, UserRecord userRecord)
    {
        Reference = reference;
        DateTimeRecord = dateTimeRecord;
        UserRecord = userRecord;
    }

    // 新規作成用
    protected EntityBase() { }

    protected T CreateModel(User createdBy)
    {
        Reference = new(default, Guid.NewGuid());
        DateTimeRecord = new(DateTime.UtcNow);
        UserRecord = new(new UserReference(createdBy));

        if (!CheckCanCreate(createdBy)) throw new ForbiddenException();
        return (T)this;
    }

    // 更新用
    protected void UpdateModel(User updatedBy, bool throughCheck = false)
    {
        UpdateModel(updatedBy, DateTime.UtcNow, throughCheck);
    }

    protected void UpdateModel(User updatedBy, DateTime updatedOn, bool throughCheck = false)
    {
        DateTimeRecord.ChangeUpdatedOn(updatedOn);
        UserRecord.ChangeUpdatedBy(new UserReference(updatedBy));

        if (!throughCheck && !CheckCanEdit(updatedBy))
            throw new ForbiddenException();
    }

    // Check if the user can do actions
    public virtual bool CheckCanGet(User user) => true;

    public virtual bool CheckCanCreate(User user)
        => user.Role == UserRole.Admin || CheckCanGet(user);

    public virtual bool CheckCanEdit(User user)
       => user.Role == UserRole.Admin
           || (CheckCanGet(user) && CheckCanCreate(user) && UserRecord.CreatedBy == new UserReference(user));

    public virtual bool CheckCanDelete(User user) => CheckCanEdit(user);

    protected override bool EqualsCore(T other) => Id == other.Id;
}
