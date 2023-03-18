using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;

namespace YomLog.Domain.Books.Entities;

public class Progress : EntityBase<Progress>
{
    public BookReference Book { get; } = null!;
    public BookPage BookPage { get; private set; } = null!;
    public ProgressState State { get; private set; }

    public Progress(BookReference book, BookPage page, ProgressState state)
    {
        Book = book;
        BookPage = page;
        State = state;
    }

    public static Progress Create(BookReference book, BookPage page, ProgressState state, User createdBy)
        => new Progress(book, page, state).CreateModel(createdBy);

    public void Edit(BookPage page, ProgressState state, User updatedBy)
    {
        BookPage = page;
        State = state;
        UpdateModel(updatedBy);
    }
}
