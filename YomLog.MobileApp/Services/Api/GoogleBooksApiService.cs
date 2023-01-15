using Microsoft.AspNetCore.WebUtilities;
using YomLog.BlazorShared.Services;
using YomLog.MobileApp.Entities;
using YomLog.Shared.Models;

namespace YomLog.MobileApp.Services.Api;

public class GoogleBooksApiService
{
    private readonly HttpClientWrapper _httpClientWrapper;

    public GoogleBooksApiService(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Pagination<BookInfo>> GetBookList(string query, int startIndex, int limit)
    {
        if (string.IsNullOrWhiteSpace(query) || limit <= 0)
            return new Pagination<BookInfo>(Enumerable.Empty<BookInfo>(), 0, startIndex, 1);

        var url = QueryHelpers.AddQueryString(
            "https://www.googleapis.com/books/v1/volumes",
            new Dictionary<string, string>
            {
                { "q", query },
                { "orderby", "relevance" },
                { "maxresults", limit.ToString() },
                { "startIndex", startIndex.ToString() }
            }
        );

        var data = await _httpClientWrapper.GetAsDJson(url);

        int totalItems = (int)data.TotalItems;
        if (data.Items == null)
            return new Pagination<BookInfo>(Enumerable.Empty<BookInfo>(), totalItems, startIndex, limit);

        var books = ((IEnumerable<dynamic>)data.Items)
            .Select(x => new BookInfo
            {
                Id = (string)x.Id,
                Title = (string)x.VolumeInfo.Title,
                Authors = (List<string>?)x.VolumeInfo.Authors,
                Description = (string?)x.VolumeInfo.Description,
                Url = (string?)x.VolumeInfo.InfoLink,
                Thumbnail = ((string?)(x.VolumeInfo.ImageLinks?.Thumbnail ?? x.VolumeInfo.ImageLinks?.SmallThumbnail))
                    ?.Replace("http://", "https://"),
                TotalPage = (int?)x.VolumeInfo.PageCount,
                Isbn = ((IEnumerable<dynamic>?)x.VolumeInfo.IndustryIdentifiers)?.FirstOrDefault()?.Identifier
            });

        return new Pagination<BookInfo>(books, totalItems, startIndex, limit);
    }
}