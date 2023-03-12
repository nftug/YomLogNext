using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Enums;
using YomLog.Shared.DTOs;

namespace YomLog.Domain.Books.Commands;

public class BookCommandDTO : ICommandDTO<Book>
{
    public Book? Entity { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Thumbnail { get; set; }
    public string? Isbn { get; set; }
    public BookType BookType { get; set; }
    public int? TotalPage { get; set; }
    public int? TotalKindleLocation { get; set; }
}
