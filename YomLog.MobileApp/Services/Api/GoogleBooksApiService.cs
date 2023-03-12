using Microsoft.AspNetCore.WebUtilities;
using YomLog.BlazorShared.Services;
using YomLog.Domain.Books.Commands;
using YomLog.Shared.Models;

namespace YomLog.MobileApp.Services.Api;

public class GoogleBooksApiService
{
    private readonly HttpClientWrapper _httpClientWrapper;

    public GoogleBooksApiService(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Pagination<BookCommandDTO>> GetBookList(string query, int startIndex, int limit)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new Pagination<BookCommandDTO>(Enumerable.Empty<BookCommandDTO>(), 0, startIndex, 1);

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
            return new Pagination<BookCommandDTO>(Enumerable.Empty<BookCommandDTO>(), totalItems, startIndex, limit);

        var books = ((IEnumerable<dynamic>)data.Items)
            .Select(x => new BookCommandDTO
            {
                Id = (string)x.Id,
                Title = (string)x.VolumeInfo.Title,
                Authors = (List<string>?)x.VolumeInfo.Authors ?? new(),
                Description = (string?)x.VolumeInfo.Description,
                Url = (string?)x.VolumeInfo.CommandLink,
                Thumbnail = ((string?)(x.VolumeInfo.ImageLinks?.Thumbnail ?? x.VolumeInfo.ImageLinks?.SmallThumbnail))
                    ?.Replace("http://", "https://"),
                TotalPage = (int?)x.VolumeInfo.PageCount,
                Isbn = ((IEnumerable<dynamic>?)x.VolumeInfo.IndustryIdentifiers)?.FirstOrDefault()?.Identifier
            });

        return new Pagination<BookCommandDTO>(books, totalItems, startIndex, limit);
    }
}