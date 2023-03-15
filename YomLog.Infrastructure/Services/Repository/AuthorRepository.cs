using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.Repository;

[InjectAsTransient]
public class AuthorRepository : RepositoryBase<Author, AuthorEDM>, IAuthorRepository
{
    public AuthorRepository(DataContext context, IQueryFactory<Author, AuthorEDM> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<List<Author>> FindAllByNameAsync(IEnumerable<AuthorName> authorNames)
        => FindAllByPredicateAsync(x => authorNames.Select(x => x.Value).Contains(x.Name));

    public Task<Author?> FindByNameAsync(AuthorName authorName)
        => FindByPredicateAsync(x => x.Name == authorName.Value);
}
