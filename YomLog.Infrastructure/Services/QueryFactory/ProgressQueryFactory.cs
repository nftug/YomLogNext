using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Extensions;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class ProgressQueryFactory : QueryFactoryBase<Progress, ProgressEDM>
{
    public ProgressQueryFactory(DataContext context) : base(context)
    {
    }

    public override IQueryable<ProgressEDM> Source
        => _context.SetWithProperties<ProgressEDM>()
            .SelectWith(x => x.Book, x => new()
            {
                Id = x.Id,
                TotalPage = x.TotalPage,
                TotalKindleLocation = x.TotalKindleLocation
            })
            .ToQueryable()
            .OrderBy(x => x.CreatedOn);
}
