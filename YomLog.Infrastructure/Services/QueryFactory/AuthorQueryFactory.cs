using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.EDMs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class AuthorQueryFactory : QueryFactoryBase<Author, AuthorEDM>
{
    public AuthorQueryFactory(DataContext context) : base(context)
    {
    }
}
