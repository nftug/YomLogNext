using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.ValueObjects;

public record BookPage
{
    public Page Page { get; }
    public KindleLocation? KindleLocation { get; }
    public BookPage? Total { get; }

    public double Percentage => Total is null ? 0 : Page.Value / Total.Page.Value;

    // 全ページ
    public BookPage(int page, int? kindleLocation)
    {
        if (page == default)
            throw new EntityValidationException("ページ数を指定してください。");
        if (page <= 0)
            throw new EntityValidationException("ページ数は0より大きな数字を指定してください。");
        if (kindleLocation <= 0)
            throw new EntityValidationException("位置Noは0より大きな数字を指定してください。");

        Page = new(page);
        KindleLocation = KindleLocation.Create(kindleLocation);
    }

    // ページ数のみ指定
    public BookPage(Page page, BookPage total) : this(page.Value, null)
    {
        Total = total;
    }

    // 位置Noを指定
    public BookPage(KindleLocation kindleLocation, BookPage total)
    {
        if (total.KindleLocation is null) throw new InvalidOperationException();

        KindleLocation = kindleLocation;
        Page = new(kindleLocation.Value / total.KindleLocation.Value);
        Total = total;
    }

    public static BookPage operator +(BookPage left, BookPage right)
    {
        var page = new Page(left.Page.Value + right.Page.Value);
        return left.KindleLocation is not null && right.KindleLocation is not null
            ? new(page.Value, left.KindleLocation.Value + right.KindleLocation.Value)
            : new(page.Value, null);
    }

    public static BookPage operator -(BookPage left, BookPage right)
    {
        var page = new Page(left.Page.Value - right.Page.Value);
        return left.KindleLocation is not null && right.KindleLocation is not null
            ? new(page.Value, left.KindleLocation.Value - right.KindleLocation.Value)
            : new(page.Value, null);
    }
}

public record Page(int Value);

public record KindleLocation(int Value)
{
    public static KindleLocation? Create(int? value)
        => value is int v ? new(v) : null;
}