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

    public Progress(
        BookReference book,
        int page,
        int? kindleLocation,
        ProgressState state,
        bool skipValidation = false
    )
    {
        Book = book;
        BookPage = kindleLocation is int kl
            ? BookPage.CreateWithKindleLocation(kl, book.TotalPage, skipValidation)
            : BookPage.CreateWithPage(page, book.TotalPage, skipValidation);
        State = state;

        if (!skipValidation) ValidateBookType();
    }

    public static Progress Create(BookReference book, int page, int? kindleLocation, ProgressState state, User createdBy)
        => new Progress(book, page, kindleLocation, state).CreateModel(createdBy);

    public void Edit(int page, int? kindleLocation, ProgressState state, User updatedBy)
    {
        BookPage = new Progress(Book, page, kindleLocation, state).BookPage;
        State = state;
        UpdateModel(updatedBy);
    }

    private void ValidateBookType()
    {
        if (BookPage.BookType != Book.BookType)
            throw new EntityValidationException(nameof(BookType), "invalid progress status (different book type)");
    }
}
