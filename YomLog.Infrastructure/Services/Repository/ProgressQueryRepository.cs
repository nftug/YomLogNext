using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.Repository;

public class ProgressQueryRepository : RepositoryBase<Progress, ProgressEDM>
{
    public ProgressQueryRepository(DataContext context, IQueryFactory<Progress, ProgressEDM> queryFactory)
        : base(context, queryFactory)
    {
    }
}
