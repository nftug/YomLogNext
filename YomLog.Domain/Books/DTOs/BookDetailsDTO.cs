using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.DTOs;

namespace YomLog.Domain.Books.DTOs;

public class BookDetailsDTO : EntityDetailsDTOBase<Book, BookDetailsDTO>
{
    public string Title { get; set; } = string.Empty;
    public string GoogleBooksId { get; set; } = string.Empty;
    public IReadOnlyList<string> Authors { get; set; } = null!;
    public string? Description { get; set; }
    public string? GoogleBooksUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Isbn { get; set; }
    public BookType BookType { get; set; }
    public BookPageDTO Total { get; set; } = null!;

    public IReadOnlyList<ProgressDetailsDTO> ProgressList { get; set; } = null!;
    public ProgressDetailsDTO? CurrentProgress { get; set; }


    public BookDetailsDTO() { }

    public BookDetailsDTO(Book model) : base(model)
    {
        Title = model.Name;
        GoogleBooksId = model.GoogleBooksId;
        Authors = model.Authors.Select(x => x.Name.Value).ToList();
        Description = model.Description;
        GoogleBooksUrl = model.GoogleBooksUrl?.ToString();
        ThumbnailUrl = model.ThumbnailUrl?.ToString();
        Isbn = model.Isbn;
        BookType = model.BookType;
        Total = new(model.TotalPage);

        ProgressList = model.Progress
            .GroupJoin(
                ProgressDiff.GetProgressDiffList(model.Progress),
                p => p.Id,
                d => d.ProgressId, (p, d) => new { p, d }
            )
            .SelectMany(
                x => x.d.DefaultIfEmpty(),
                (x, d) => new ProgressDetailsDTO(x.p, d ?? new ProgressDiff(x.p.BookPage, x.p.Id))
            )
            .ToList();

        CurrentProgress = ProgressDetailsDTO.Create(model.CurrentProgress, null);
    }

    public override ICommandDTO<Book> ToCommandDTO()
        => new BookCommandDTO
        {
            Id = GoogleBooksId,
            Title = Title,
            Authors = Authors.ToList(),
            Description = Description,
            Url = GoogleBooksUrl,
            Thumbnail = ThumbnailUrl,
            Isbn = Isbn,
            TotalPage = Total.Page,
            TotalKindleLocation = Total.KindleLocation,
        };

    public override string ToString() => Title;
}