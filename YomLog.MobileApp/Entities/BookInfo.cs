namespace YomLog.MobileApp.Entities;

public class BookInfo
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public List<string>? Authors { get; init; } = new();
    public string? Description { get; init; } = string.Empty;
    public string? Url { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    public int? TotalPage { get; init; }
    public string? Isbn { get; init; }
}
