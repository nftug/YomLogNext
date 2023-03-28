using YomLog.Domain.Books.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public class BookReference : EntityReference<Book>
{
    public BookPage TotalPage { get; }

    public BookReference(long pk, Guid id, BookPage totalPage) : base(pk, id)
    {
        TotalPage = totalPage;
    }

    public BookReference(Book origin) : base(origin)
    {
        TotalPage = origin.TotalPage;
    }
}
