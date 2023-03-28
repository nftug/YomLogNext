using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Extensions;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class BookQueryFactory : QueryFactoryBase<Book, BookEDM>
{
    public BookQueryFactory(DataContext context) : base(context)
    {
    }

    public override IQueryable<BookEDM> Source
        => _context.SetWithProperties<BookEDM>()
            .SelectWith(x => new()
            {
                Authors = x.Authors
                    .Select(x => new AuthorEDM
                    {
                        PK = x.PK,
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToList(),
                Progress = x.Progress
                    .Select(p => new ProgressEDM
                    {
                        Id = p.Id,
                        FKBook = p.FKBook,
                        Page = p.Page,
                        KindleLocation = p.KindleLocation,
                        State = p.State,
                        CreatedOn = p.CreatedOn,
                        UpdatedOn = p.UpdatedOn
                    })
                    .ToList()
            })
            .ToQueryable()
            .OrderBy(x => x.PK);

    public override IQueryable<BookEDM> ListSource
        => base.Source
            .OrderByDescending(x => x.PK)
#pragma warning disable CS8073
            .ThenBy(x => x.Progress.First().CreatedOn == null)
            .ThenByDescending(x => x.Progress.First().CreatedOn);
#pragma warning restore CS8073
}
