using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public class BookReference : EntityReference<Book>
{
    public BookPage TotalPage { get; }
    public BookType BookType => TotalPage.BookType;

    public BookReference(long pk, Guid id, BookPage totalPage) : base(pk, id)
    {
        TotalPage = totalPage;
    }

    public BookReference(Book origin) : base(origin)
    {
        TotalPage = origin.TotalPage;
    }
}
