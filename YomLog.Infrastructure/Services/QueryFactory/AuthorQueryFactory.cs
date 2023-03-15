using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Services.QueryFactory;

[InjectAsTransient]
public class AuthorQueryFactory : QueryFactoryBase<Author, AuthorEDM>
{
    public AuthorQueryFactory(DataContext context) : base(context)
    {
    }
}
