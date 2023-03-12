using YomLog.Domain.Books.Entities;
using YomLog.Domain.ValueObjects;
using YomLog.Shared.Interfaces;

namespace YomLog.Domain.Books.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> FindByNameAsync(AuthorName name);
    Task<List<Author>> FindAllByNameAsync(IEnumerable<AuthorName> names);
}
