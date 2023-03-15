using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.Shared.DAOs;

namespace YomLog.Infrastructure.DAOs;

public class AuthorDAO : EntityDAOBase<Author, AuthorDAO>
{
    public string Name { get; set; } = string.Empty;

    protected override Author PrepareDomainEntity() => new(new(Name));

    internal override AuthorDAO Transfer(Author origin)
    {
        Name = origin.Name.Value;
        return base.Transfer(origin);
    }
}
