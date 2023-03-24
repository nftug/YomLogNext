using System.Text.Json.Serialization;
using YomLog.Domain.Books.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public class ProgressDiff : ValueObject<ProgressDiff>
{
    public BookPage Value { get; init; } = null!;
    public double Percentage { get; init; }

    [JsonConstructor] public ProgressDiff() { }

    public ProgressDiff(Progress prev, Progress current, BookPage totalPage)
    {
        Value = current.BookPage - prev.BookPage;
        Percentage = (double)Value.Page / totalPage.Page;
    }

    protected override bool EqualsCore(ProgressDiff other)
        => Value == other.Value && Percentage == other.Percentage;

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
