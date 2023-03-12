using YomLog.Shared.Exceptions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.Entities;

public class User : EntityWithNameBase<User>
{
    public UserRole Role { get; private set; }
    public EntityReferenceWithName<User> UserReference => new(Id, Name);

    public User(string name, UserRole role) : base(name)
    {
        Role = role;
    }

    public static User GetDummyUser()
    {
        var dummyReference = new EntityReferenceWithName<User>(Guid.Empty, "User");
        return new User(string.Empty, UserRole.User)
        {
            Reference = dummyReference,
            UserRecord = new(dummyReference),
            DateTimeRecord = new(DateTime.Now)
        };
    }

    public static User CreateNew(string name, UserRole role, User createdBy)
        => new User(name, role).CreateModel(createdBy);

    // Should be called after validating name
    internal void Edit(string name, UserRole role, User updatedBy)
    {
        if (role != Role && updatedBy == this)
            throw new EntityValidationException(nameof(Role), "自分自身のロールを変更することはできません。");

        Name = name;
        Role = role;
        UpdateModel(updatedBy);
    }

    internal void Edit(string name, User updatedBy)
    {
        Name = name;
        UpdateModel(updatedBy);
    }
}

public enum UserRole
{
    User,
    Admin
}
