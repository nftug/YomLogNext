using YomLog.Domain.Books.Enums;
using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.ValueObjects;

public record BookPage
{
    public int Page { get; }
    public int? KindleLocation { get; }
    public BookPage? Total { get; private init; }

    public BookType BookType => KindleLocation is not null ? BookType.Kindle : BookType.Normal;
    public double? Percentage => Total is not null ? (double)Page / Total.Page : null;

    // 全ページ
    public BookPage(int page, int? kindleLocation, bool skipValidation = false)
    {
        if (!skipValidation)
        {
            if (page == default)
                throw new EntityValidationException("ページ数を指定してください。");
            if (page <= 0)
                throw new EntityValidationException("ページ数は0より大きな数字を指定してください。");
            if (kindleLocation <= 0)
                throw new EntityValidationException("位置Noは0より大きな数字を指定してください。");
        }

        Page = page;
        KindleLocation = kindleLocation;
    }

    /// <summary>
    /// Create instance with page (for data retrieved from repository)
    /// </summary>
    /// <param name="page"></param>
    /// <param name="total"></param>
    /// <returns></returns>
    public static BookPage CreateWithPage(int page, BookPage total)
        => new(page, null, true) { Total = total };

    /// <summary>
    /// Create instance with kindle location (for data retrieved from repository)
    /// </summary>
    /// <param name="page"></param>
    /// <param name="total"></param>
    /// <returns></returns>
    public static BookPage CreateWithKindleLocation(int kl, BookPage total)
        => new(kl / total.KindleLocation!.Value, kl, true) { Total = total };

    public static BookPage operator +(BookPage left, BookPage right)
    {
        var page = left.Page + right.Page;
        return left.KindleLocation + right.KindleLocation is int kl
            ? CreateWithKindleLocation(kl, left.Total!)
            : CreateWithPage(page, left.Total!);
    }

    public static BookPage operator -(BookPage left, BookPage right)
    {
        var page = left.Page - right.Page;
        return left.KindleLocation - right.KindleLocation is int kl
            ? CreateWithKindleLocation(kl, left.Total!)
            : CreateWithPage(page, left.Total!);
    }
}