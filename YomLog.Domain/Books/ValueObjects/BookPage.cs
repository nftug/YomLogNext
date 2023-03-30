using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.ValueObjects;

public record BookPage(int Page, int? KindleLocation, double Percentage)
{
    // 全ページ
    public BookPage(int page, int? kindleLocation, bool skipValidation = false)
        : this(page > 0 ? page : 0, kindleLocation > 0 ? kindleLocation : 0, 1.0)
    {
        if (skipValidation) return;

        if (page == default && kindleLocation is null)
            throw new EntityValidationException("ページ数か位置Noのいずれかを指定してください。");
        if (page == default && kindleLocation is null)
            throw new EntityValidationException("ページ数は0より大きな数字を指定してください。");
        if (kindleLocation == 0 && page == default)
            throw new EntityValidationException("位置Noは0より大きな数字を指定してください。");
    }

    // ページ数のみ指定
    public BookPage(int page, BookPage totalBookPage) : this(page, null, (double)page / totalBookPage.Page)
    {
    }

    // 位置Noを指定
    public BookPage(int? kindleLocation, BookPage totalBookPage) : this(0, null!)
    {
        if (kindleLocation is not int location) throw new InvalidOperationException();

        KindleLocation = location;
        Page = location / (int)totalBookPage.KindleLocation! * totalBookPage.Page;
        Percentage = (double)location / (int)totalBookPage.KindleLocation;
    }

    public static BookPage operator +(BookPage left, BookPage right)
        => new(left.Page + right.Page, left.KindleLocation + right.KindleLocation);

    public static BookPage operator -(BookPage left, BookPage right)
        => new(left.Page - right.Page, left.KindleLocation - right.KindleLocation);
}
