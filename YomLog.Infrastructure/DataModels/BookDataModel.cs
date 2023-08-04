using LiteDB;
using YomLog.Domain.Books.Entities;
using YomLog.Infrastructure.Shared.DataModels;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.DataModels;

public class BookDataModel : DataModelBase<Book, BookDataModel>
{
    public string GoogleBooksId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [BsonRef("Authors")] public List<AuthorDataModel> Authors { get; set; } = new();
    public string? Description { get; set; }
    public string? GoogleBooksUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Isbn { get; set; }
    public int TotalPage { get; set; }
    public int? TotalKindleLocation { get; set; }

    public List<ProgressDataModel> Progress { get; set; } = new();

    protected override Book PrepareDomainEntity()
        => new(
            googleBooksId: GoogleBooksId,
            name: Name,
            authors: Authors.Select(x => x.ToDomain()).ToList(),
            description: Description,
            googleBooksUrl: GoogleBooksUrl != null ? new(GoogleBooksUrl) : null,
            thumbnailUrl: ThumbnailUrl != null ? new(ThumbnailUrl) : null,
            isbn: Isbn,
            totalPage: new(TotalPage, TotalKindleLocation, true),
            progress: Progress.Select(x => x.ToDomain(this)).ToList()
        );

    internal override BookDataModel Transfer(Book origin)
    {
        GoogleBooksId = origin.GoogleBooksId;
        Name = origin.Name;
        Description = origin.Description;
        GoogleBooksUrl = origin.GoogleBooksUrl?.AbsoluteUri;
        ThumbnailUrl = origin.ThumbnailUrl?.AbsoluteUri;
        Isbn = origin.Isbn;
        TotalPage = origin.TotalPage.Page;
        TotalKindleLocation = origin.TotalPage.KindleLocation;

        Progress = origin.Progress
            .Select(x => new ProgressDataModel().Transfer(x))
            .ToList();
        Authors = origin.Authors.Select(x => new AuthorDataModel { PK = x.PK }).ToList();

        return base.Transfer(origin);
    }
}
