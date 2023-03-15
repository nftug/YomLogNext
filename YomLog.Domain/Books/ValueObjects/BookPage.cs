using YomLog.Shared.Exceptions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public class BookPage : ValueObject<BookPage>
{
    public int? Page { get; }
    public int? KindleLocation { get; }

    public BookPage(int? page, int? kindleLocation, bool skipValidation = false)
    {
        Page = page;
        KindleLocation = kindleLocation;

        if (skipValidation) return;

        if (page is null && kindleLocation is null)
            throw new EntityValidationException("ページ数か位置Noのいずれかを指定してください。");
        if (page < 0 || (page == 0 && kindleLocation is null))
            throw new EntityValidationException("ページ数は0より大きな数字を指定してください。");
        if (KindleLocation < 0 || (kindleLocation == 0 && page is null))
            throw new EntityValidationException("位置Noは0より大きな数字を指定してください。");
    }

    protected override bool EqualsCore(BookPage other)
        => Page == other.Page && KindleLocation == other.KindleLocation;
}
