using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Infrastructure.DataModels;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class BookRepository : RepositoryBase<Book, BookDataModel>, IBookRepository
{
    public BookRepository(DataContext context, IQueryFactory<Book, BookDataModel> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<Book?> FindByGoogleBooksIdAsync(string googleBooksId)
        => FindByPredicateAsync(x => x.GoogleBooksId == googleBooksId);
}
