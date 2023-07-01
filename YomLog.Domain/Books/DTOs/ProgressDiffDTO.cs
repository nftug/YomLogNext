using YomLog.Domain.Books.ValueObjects;

namespace YomLog.Domain.Books.DTOs;

public record ProgressDiffDTO(BookPageDTO Position, double Percentage)
{
    public ProgressDiffDTO(ProgressDiff origin)
        : this(new(origin.Value), origin.Percentage)
    {
    }

    public static ProgressDiffDTO? Create(ProgressDiff? origin)
        => origin is not null ? new(origin) : null;
}