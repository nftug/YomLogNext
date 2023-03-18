using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.DTOs;

namespace YomLog.Domain.Books.DTOs;

public class ProgressDetailsDTO : EntityDetailsDTOBase<Progress, ProgressDetailsDTO>
{
    public Guid BookId { get; set; }
    public BookPage BookPage { get; set; }
    public ProgressState State { get; set; }
    public ProgressDiff Diff { get; set; }

    public ProgressDetailsDTO(Progress currentProgress, ProgressDiff progressDiff)
    {
        BookId = currentProgress.Book.Id;
        BookPage = currentProgress.BookPage;
        State = currentProgress.State;
        Diff = progressDiff;
    }

    public override ICommandDTO<Progress> ToCommandDTO()
        => new ProgressCommandDTO
        {
            BookId = BookId,
            State = State,
            Page = BookPage.Page,
            KindleLocation = BookPage.KindleLocation
        };

    public override string ToString() => Id.ToString();
}
