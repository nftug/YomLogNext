using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Interfaces;

namespace YomLog.Domain.Books.Interfaces;

public interface IProgressRepository : IRepository<Progress>
{
    Task<List<Progress>> FindAllByBook(BookReference book);
}
