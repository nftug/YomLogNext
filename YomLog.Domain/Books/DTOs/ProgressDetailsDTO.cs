using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.DTOs;

namespace YomLog.Domain.Books.DTOs;

public class ProgressDetailsDTO : EntityDetailsDTOBase<Progress, ProgressDetailsDTO>
{
    public Guid BookId { get; set; }
    public BookPageDTO Position { get; set; }
    public ProgressState State { get; set; }
    public ProgressDiffDTO? Diff { get; set; }

    public ProgressDetailsDTO(Progress currentProgress, ProgressDiff? progressDiff)
    {
        BookId = currentProgress.Book.Id;
        Position = new(currentProgress.BookPage);
        State = currentProgress.State;
        Diff = ProgressDiffDTO.Create(progressDiff);
    }

    public override ICommandDTO<Progress> ToCommandDTO()
        => new ProgressCommandDTO
        {
            BookId = BookId,
            State = State,
            Page = Position.Page,
            KindleLocation = Position.KindleLocation,
        };

    public override string ToString() => Id.ToString();

    public static ProgressDetailsDTO? Create(Progress? currentProgress, ProgressDiff? progressDiff)
        => currentProgress is not null ? new(currentProgress, progressDiff) : null;
}
