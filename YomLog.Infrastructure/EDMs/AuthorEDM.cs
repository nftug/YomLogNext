using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.Shared.EDMs;

namespace YomLog.Infrastructure.EDMs;

public class AuthorEDM : EntityEDMBase<Author, AuthorEDM>
{
    public string Name { get; set; } = string.Empty;

    protected override Author PrepareDomainEntity() => new(new(Name));

    internal override AuthorEDM Transfer(Author origin)
    {
        Name = origin.Name.Value;
        return base.Transfer(origin);
    }
}
