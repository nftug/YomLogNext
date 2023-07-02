using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;

namespace YomLog.Domain.Books.Entities;

public class Book : EntityWithNameBase<Book>
{
    public string GoogleBooksId { get; set; } = string.Empty;
    public IReadOnlyList<Author> Authors { get; private set; } = null!;
    public string? Description { get; private set; }
    public Uri? GoogleBooksUrl { get; private set; }
    public Uri? ThumbnailUrl { get; private set; }
    public string? Isbn { get; private set; }
    public BookPage TotalPage { get; private set; } = null!;
    public Progress? CurrentProgress { get; }

    public BookType BookType => TotalPage.BookType;

    public Book(
        string googleBooksId,
        string name,
        IReadOnlyList<Author> authors,
        string? description,
        Uri? googleBooksUrl,
        Uri? thumbnailUrl,
        string? isbn,
        BookPage totalPage,
        Progress? currentProgress
    ) : base(name)
    {
        GoogleBooksId = googleBooksId;
        Authors = authors;
        Description = description;
        GoogleBooksUrl = googleBooksUrl;
        ThumbnailUrl = thumbnailUrl;
        Isbn = isbn;
        TotalPage = totalPage;
        CurrentProgress = currentProgress;
    }

    public static Book Create(BookCommandDTO command, IReadOnlyList<Author> authors, User createdBy)
        => new Book(
            command.Id,
            command.Title,
            authors,
            command.Description,
            command.Url != null ? new(command.Url) : null,
            command.Thumbnail != null ? new(command.Thumbnail) : null,
            command.Isbn,
            new(command.TotalPage, command.TotalKindleLocation),
            null
        )
        .CreateModel(createdBy);

    public void Edit(BookCommandDTO command, IReadOnlyList<Author> authors, User updatedBy)
    {
        Name = command.Title;
        Authors = authors;
        TotalPage =
            BookType == BookType.Normal
            ? new(command.TotalPage, null)
            : new(TotalPage.Page, command.TotalKindleLocation);
        UpdateModel(updatedBy);
    }
}
