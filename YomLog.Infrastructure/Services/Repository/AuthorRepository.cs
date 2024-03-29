using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Infrastructure.DataModels;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class AuthorRepository : RepositoryBase<Author, AuthorDataModel>, IAuthorRepository
{
    public AuthorRepository(AppConfig appConfig) : base(appConfig)
    {
    }

    public Task<List<Author>> FindAllByNameAsync(IEnumerable<AuthorName> authorNames)
        => FindAllByPredicateAsync(x => authorNames.Select(x => x.Value).Contains(x.Name));

    public Task<Author?> FindByNameAsync(AuthorName authorName)
        => FindByPredicateAsync(x => x.Name == authorName.Value);
}
