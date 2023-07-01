using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class ProgressRepository : RepositoryBase<Progress, ProgressEDM>, IProgressRepository
{
    public ProgressRepository(DataContext context, IQueryFactory<Progress, ProgressEDM> queryFactory)
        : base(context, queryFactory)
    {
    }

    public Task<List<Progress>> FindAllByBook(BookReference book)
        => FindAllByPredicateAsync(x => x.Book.Id == book.Id);
}
