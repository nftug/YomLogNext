using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.DAOs;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class AuthorQueryFactory : QueryFactoryBase<Author, AuthorDAO>
{
    public AuthorQueryFactory(DataContext context) : base(context)
    {
    }
}
