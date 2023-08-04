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

    private readonly List<Progress> _progress = new();
    public IReadOnlyList<Progress> Progress => _progress;

    public BookType BookType => TotalPage.BookType;
    public Progress? CurrentProgress
        => Progress.OrderByDescending(x => x.DateTimeRecord.CreatedOn).FirstOrDefault();

    public Book(
        string googleBooksId,
        string name,
        IReadOnlyList<Author> authors,
        string? description,
        Uri? googleBooksUrl,
        Uri? thumbnailUrl,
        string? isbn,
        BookPage totalPage,
        IReadOnlyList<Progress> progress
    ) : base(name)
    {
        GoogleBooksId = googleBooksId;
        Authors = authors;
        Description = description;
        GoogleBooksUrl = googleBooksUrl;
        ThumbnailUrl = thumbnailUrl;
        Isbn = isbn;
        TotalPage = totalPage;
        _progress = progress.ToList();
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
            new List<Progress>()
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

    public void AddProgress(Progress prog)
    {
        _progress.Add(prog);
    }

    public void DeleteProgress(Guid progId)
    {
        var item = _progress.First(x => x.Id == progId);
        _progress.Remove(item);
    }
}
