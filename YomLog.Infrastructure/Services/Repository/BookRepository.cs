using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Infrastructure.DAOs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class BookRepository : RepositoryBase<Book, BookDAO>, IBookRepository
{
    public BookRepository(DataContext context, IQueryFactory<Book, BookDAO> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<Book?> FindByGoogleBooksIdAsync(string googleBooksId)
        => FindByPredicateAsync(x => x.GoogleBooksId == googleBooksId);
}
