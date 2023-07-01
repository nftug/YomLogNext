using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.Entities;

public class Progress : EntityBase<Progress>
{
    public BookReference Book { get; } = null!;
    public BookPage BookPage { get; private set; } = null!;
    public ProgressState State { get; private set; }

    // From DataModel
    public Progress(BookReference book, int page, int? kindleLocation, ProgressState state)
    {
        Book = book;
        BookPage = kindleLocation is int kl
            ? BookPage.CreateWithKindleLocation(kl, book.TotalPage)
            : BookPage.CreateWithPage(page, book.TotalPage);
        State = state;
    }

    private Progress(BookReference book, BookPage page, ProgressState state)
    {
        Book = book;
        BookPage = page;
        State = state;
        ValidateBookType();
    }

    public static Progress Create(BookReference book, BookPage page, ProgressState state, User createdBy)
        => new Progress(book, page, state).CreateModel(createdBy);

    public void Edit(BookPage page, ProgressState state, User updatedBy)
    {
        BookPage = page;
        State = state;
        ValidateBookType();
        UpdateModel(updatedBy);
    }

    private void ValidateBookType()
    {
        if (BookPage.BookType != Book.BookType)
            throw new EntityValidationException(nameof(BookType), "invalid progress status (different book type)");
    }
}
