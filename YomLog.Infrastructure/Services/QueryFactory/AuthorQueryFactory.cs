using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.DataModels;
using YomLog.Infrastructure.Shared.Services;

namespace YomLog.Infrastructure.Services.QueryFactory;

public class AuthorQueryFactory : QueryFactoryBase<Author, AuthorDataModel>
{
    public AuthorQueryFactory(DataContext context) : base(context)
    {
    }
}
