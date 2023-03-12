using YomLog.Domain.Books.Entities;
using YomLog.Shared.Interfaces;

namespace YomLog.Domain.Books.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> FindByGoogleBooksIdAsync(string googleBooksId);
}
