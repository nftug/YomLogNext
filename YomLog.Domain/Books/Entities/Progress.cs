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
            ? new BookPage(new KindleLocation(kl), book.TotalPage)
            : new BookPage(new Page(page), book.TotalPage);
        State = state;
    }

    private Progress(BookReference book, BookPage page, ProgressState state)
    {
        Book = book;
        BookPage = page;
        State = state;
        ValidateBookPage();
    }

    public static Progress Create(BookReference book, BookPage page, ProgressState state, User createdBy)
        => new Progress(book, page, state).CreateModel(createdBy);

    public void Edit(BookPage page, ProgressState state, User updatedBy)
    {
        BookPage = page;
        State = state;
        ValidateBookPage();
        UpdateModel(updatedBy);
    }

    private void ValidateBookPage()
    {
        if (BookPage.BookType != Book.BookType)
            throw new EntityValidationException(nameof(BookType), "invalid status (different book type)");
    }
}
