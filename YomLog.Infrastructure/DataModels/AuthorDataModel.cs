using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.Shared.DataModels;

namespace YomLog.Infrastructure.DataModels;

public class AuthorDataModel : DataModelBase<Author, AuthorDataModel>
{
    public string Name { get; set; } = string.Empty;

    protected override Author PrepareDomainEntity() => new(new(Name));

    internal override AuthorDataModel Transfer(Author origin)
    {
        Name = origin.Name.Value;
        return base.Transfer(origin);
    }
}
