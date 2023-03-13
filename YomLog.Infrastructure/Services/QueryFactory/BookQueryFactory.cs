using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.DataModels;
using YomLog.Infrastructure.Shared.Extensions;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class BookQueryFactory : QueryFactoryBase<Book, BookDataModel>
{
    public BookQueryFactory(DataContext context) : base(context)
    {
    }

    public override IQueryable<BookDataModel> Source
        => _context.SetWithProperties<BookDataModel>()
            .SelectWith(x => new()
            {
                Authors = x.Authors
                    .Select(x => new AuthorDataModel
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
    public override IQueryable<BookDataModel> ListSource
        => base.Source.OrderByDescending(x => x.PK);
}
