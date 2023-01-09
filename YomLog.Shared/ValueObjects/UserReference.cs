using System.Text.Json.Serialization;
using YomLog.Shared.Entities;

namespace YomLog.Shared.ValueObjects;

public class UserReference : ValueObject<UserReference>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public UserReference(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public UserReference(UserReference origin)
    {
        Id = origin.Id;
        Name = origin.Name;
    }

    public UserReference(User user)
    {
        Id = user.Id;
        Name = user.Name;
    }

    [JsonConstructor]
    public UserReference() { }

    protected override bool EqualsCore(UserReference other) => Id == other.Id;
}