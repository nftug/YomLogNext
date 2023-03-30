using YomLog.Domain.Books.Entities;

namespace YomLog.Domain.Books.ValueObjects;

public record ProgressDiff(BookPage Value, double Percentage)
{
    public ProgressDiff(Progress prev, Progress current, BookPage totalPage)
        : this(current.BookPage - prev.BookPage, 0)
    {
        Percentage = (double)Value.Page / totalPage.Page;
    }

    public static IEnumerable<ProgressDiff> GetProgressDiffList(
        IReadOnlyList<Progress> source,
        BookReference book
    )
        => source.Any()
            ? Enumerable.Range(0, source.Count - 1)
                .Select(i => source.Skip(i).Take(2))
                .Select(x => new ProgressDiff(x.First(), x.Last(), book.TotalPage))
            : Enumerable.Empty<ProgressDiff>();
}
