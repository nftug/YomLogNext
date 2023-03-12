using Microsoft.EntityFrameworkCore;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Infrastructure.Shared.DataModels;

namespace YomLog.Infrastructure.DataModels;

public class BookDataModel : DataModelBase<Book, BookDataModel>
{
    public string GoogleBooksId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<AuthorDataModel> Authors { get; set; } = null!;
    public string? Description { get; set; }
    public string? GoogleBooksUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Isbn { get; set; }
    public BookType BookType { get; set; }
    public int? TotalPage { get; set; }
    public int? TotalKindleLocation { get; set; }

    public ICollection<BookAuthorDataModel> BookAuthors { get; set; } = null!;

    protected override Book PrepareDomainEntity()
        => new(
            googleBooksId: GoogleBooksId,
            name: Name,
            authors: Authors.Select(x => x.ToDomain()).ToList(),
            description: Description,
            googleBooksUrl: GoogleBooksUrl != null ? new(GoogleBooksUrl) : null,
            thumbnailUrl: ThumbnailUrl != null ? new(ThumbnailUrl) : null,
            isbn: Isbn,
            bookType: BookType,
            totalPage: new(TotalPage, TotalKindleLocation)
        );

    internal override BookDataModel Transfer(Book origin)
    {
        GoogleBooksId = origin.GoogleBooksId;
        Name = origin.Name;
        Description = origin.Description;
        GoogleBooksUrl = origin.GoogleBooksUrl?.AbsoluteUri;
        ThumbnailUrl = origin.ThumbnailUrl?.AbsoluteUri;
        Isbn = origin.Isbn;
        BookType = origin.BookType;
        TotalPage = origin.TotalPage.Page;
        TotalKindleLocation = origin.TotalPage.KindleLocation;
        return base.Transfer(origin);
    }

    internal override void ApplyNavigation(Book model)
    {
        BookAuthors = model.Authors
            .Select(x => new BookAuthorDataModel { FKBook = PK, FKAuthor = x.PK })
            .ToList();
    }

    protected override void OnBuildEdm(ModelBuilder modelBuilder)
    {
        base.OnBuildEdm(modelBuilder);

        modelBuilder.Entity<BookDataModel>()
            .HasMany(x => x.Authors)
            .WithMany()
            .UsingEntity<BookAuthorDataModel>(
                r => r
                    .HasOne(r => r.Author)
                    .WithMany()
                    .HasForeignKey(r => r.FKAuthor),
                r => r
                    .HasOne(r => r.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(x => x.FKBook)
            );
    }
}
