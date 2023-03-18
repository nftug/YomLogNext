using System.Text.Json.Serialization;
using YomLog.Shared.Exceptions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public class BookPage : ValueObject<BookPage>
{
    public int Page { get; init; }
    public int? KindleLocation { get; init; }
    public double Percentage { get; init; }

    [JsonConstructor] public BookPage() { }

    // 全ページ
    public BookPage(int page, int? kindleLocation, bool skipValidation = false)
    {
        Page = page > 0 ? page : 0;
        KindleLocation = kindleLocation > 0 ? kindleLocation : 0;
        Percentage = 1.0;

        if (skipValidation) return;

        if (page == default && kindleLocation is null)
            throw new EntityValidationException("ページ数か位置Noのいずれかを指定してください。");
        if (page == default && kindleLocation is null)
            throw new EntityValidationException("ページ数は0より大きな数字を指定してください。");
        if (kindleLocation == 0 && page == default)
            throw new EntityValidationException("位置Noは0より大きな数字を指定してください。");
    }

    // ページ数のみ指定
    public BookPage(int page, BookPage totalBookPage)
    {
        Page = page;
        Percentage = (double)page / totalBookPage.Page;
    }

    // 位置Noを指定
    public BookPage(int? kindleLocation, BookPage totalBookPage)
    {
        if (kindleLocation is not int location) throw new InvalidOperationException();

        KindleLocation = location;
        Page = location / (int)totalBookPage.KindleLocation! * totalBookPage.Page;
        Percentage = (double)location / (int)totalBookPage.KindleLocation;
    }

    protected override bool EqualsCore(BookPage other)
        => Page == other.Page && KindleLocation == other.KindleLocation;

    public static BookPage operator +(BookPage left, BookPage right)
        => new(left.Page + right.Page, left.KindleLocation + right.KindleLocation);

    public static BookPage operator -(BookPage left, BookPage right)
        => new(left.Page - right.Page, left.KindleLocation - right.KindleLocation);
}
