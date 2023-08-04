using LiteDB.Async;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Infrastructure.DataModels;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class BookRepository : RepositoryBase<Book, BookDataModel>, IBookRepository
{
    public BookRepository(AppConfig appConfig) : base(appConfig)
    {
    }

    protected override ILiteQueryableAsync<BookDataModel> GetCollectionForQuery(
        ILiteCollectionAsync<BookDataModel> collection
    )
        => collection.Include(x => x.Authors).Query();

    public Task<Book?> FindByGoogleBooksIdAsync(string googleBooksId)
        => FindByPredicateAsync(x => x.GoogleBooksId == googleBooksId);
}
