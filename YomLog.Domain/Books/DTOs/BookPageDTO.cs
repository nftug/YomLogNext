using YomLog.Domain.Books.ValueObjects;

namespace YomLog.Domain.Books.DTOs;

public record BookPageDTO(int Page, int? KindleLocation, double? Percentage)
{
    public BookPageDTO(BookPage origin)
        : this(origin.Page, origin.KindleLocation, origin.Percentage)
    {
    }
}
