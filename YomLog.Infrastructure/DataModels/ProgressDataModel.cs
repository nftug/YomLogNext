using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;

namespace YomLog.Infrastructure.DataModels;

public class ProgressDataModel
{
    public int Page { get; set; }
    public int? KindleLocation { get; set; }
    public ProgressState State { get; set; }

    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    internal Progress ToDomain(BookDataModel book)
        => new Progress(
            book: new(book.PK, book.Id, new(book.TotalPage, book.TotalKindleLocation, true)),
            page: Page,
            kindleLocation: KindleLocation,
            state: State,
            skipValidation: true
        ).Transfer(
            reference: new(default, Id),
            dateTimeRecord: new(CreatedOn, UpdatedOn),
            userRecord: new(
                new(book.CreatedById, book.CreatedByName),
                book.UpdatedById != null ? new((Guid)book.UpdatedById, book.UpdatedByName!) : null
            )
        );

    internal ProgressDataModel Transfer(Progress origin)
    {
        Id = origin.Reference.Id;
        CreatedOn = origin.DateTimeRecord.CreatedOn;
        UpdatedOn = origin.DateTimeRecord.UpdatedOn;

        Page = origin.BookPage.Page;
        KindleLocation = origin.BookPage.KindleLocation;
        State = origin.State;
        return this;
    }

    public override bool Equals(object? obj) => Id == (obj as BookDataModel)?.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
