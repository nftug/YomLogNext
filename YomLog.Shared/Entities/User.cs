using YomLog.Shared.Exceptions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.Entities;

public class User : EntityBase<User>
{
    public string Name { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    public User(
        ModelReference<User> reference,
        DateTimeRecord dateTimeRecord,
        UserRecord userRecord,
        string name,
        UserRole role
    ) : base(reference, dateTimeRecord, userRecord)
    {
        Name = name;
        Role = role;
    }

    private User() { }

    public static User CreateNew(string name, UserRole role, User createdBy)
        => new User { Name = name, Role = role }.CreateModel(createdBy);

    // Should be called after validating name
    internal void Edit(string name, UserRole role, User updatedBy)
    {
        if (role != Role && updatedBy == this)
            throw new ModelErrorException(nameof(Role), "Cannot change your own role.");

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