using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Extensions;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.QueryFactory;

[InjectAsTransient]
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
                    .ToList()
            })
            .ToQueryable()
            .OrderByDescending(x => x.PK);

    // TODO: ステータスの更新日時で並び替える
    public override IQueryable<BookEDM> ListSource
        => base.Source.OrderByDescending(x => x.PK);
}
