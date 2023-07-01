using YomLog.Domain.Books.ValueObjects;

namespace YomLog.Domain.Books.DTOs;

public record BookPageDTO(int Page, int? KindleLocation)
{
    public BookPageDTO(BookPage origin) : this(origin.Page.Value, origin.KindleLocation?.Value) { }
}
