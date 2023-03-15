using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class BookRepository : RepositoryBase<Book, BookEDM>, IBookRepository
{
    public BookRepository(DataContext context, IQueryFactory<Book, BookEDM> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<Book?> FindByGoogleBooksIdAsync(string googleBooksId)
        => FindByPredicateAsync(x => x.GoogleBooksId == googleBooksId);
}
