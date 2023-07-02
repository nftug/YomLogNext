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
            System.Diagnostics.Debug.WriteLine($"page: {page}, kindleLocation: {kindleLocation}");

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

    public static BookPage CreateWithPage(int page, BookPage total, bool skipValidation = false)
        => new(page, null, skipValidation) { Total = total };

    public static BookPage CreateWithKindleLocation(int kl, BookPage total, bool skipValidation = false)
    {
        int page = (int)(kl / (double)(total.KindleLocation ?? 1) * total.Page);
        return new(page, kl, skipValidation) { Total = total };
    }

    public static BookPage operator +(BookPage left, BookPage right)
    {
        var page = left.Page + right.Page;
        return left.KindleLocation + right.KindleLocation is int kl
            ? CreateWithKindleLocation(kl, left.Total!, true)
            : CreateWithPage(page, left.Total!, true);
    }

    public static BookPage operator -(BookPage left, BookPage right)
    {
        var page = left.Page - right.Page;
        return left.KindleLocation - right.KindleLocation is int kl
            ? CreateWithKindleLocation(kl, left.Total!, true)
            : CreateWithPage(page, left.Total!, true);
    }
}