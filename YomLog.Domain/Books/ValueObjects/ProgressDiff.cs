using YomLog.Domain.Books.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.ValueObjects;

public record ProgressDiff(BookPage Value, double Percentage, Guid ProgressId)
{
    public ProgressDiff(Progress prev, Progress current)
        : this(current.BookPage - prev.BookPage, 0, current.Id)
    {
        if (prev.Book.Id != current.Book.Id)
            throw new EntityValidationException(nameof(ProgressDiff), "cannot compare different book's progress");

        Percentage = (double)Value.Page.Value / current.Book.TotalPage.Page.Value;
    }

    public static IEnumerable<ProgressDiff> GetProgressDiffList(IReadOnlyList<Progress> source)
        => source.Any()
            ? Enumerable.Range(0, source.Count - 1)
                .Select(i => source.Skip(i).Take(2))
                .Select(x => new ProgressDiff(x.First(), x.Last()))
            : Enumerable.Empty<ProgressDiff>();
}
