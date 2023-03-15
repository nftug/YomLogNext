using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Infrastructure.DAOs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class AuthorRepository : RepositoryBase<Author, AuthorDAO>, IAuthorRepository
{
    public AuthorRepository(DataContext context, IQueryFactory<Author, AuthorDAO> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<List<Author>> FindAllByNameAsync(IEnumerable<AuthorName> authorNames)
        => FindAllByPredicateAsync(x => authorNames.Select(x => x.Value).Contains(x.Name));

    public Task<Author?> FindByNameAsync(AuthorName authorName)
        => FindByPredicateAsync(x => x.Name == authorName.Value);
}
