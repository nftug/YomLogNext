using Microsoft.AspNetCore.WebUtilities;
using YomLog.BlazorShared.Services;
using YomLog.Domain.Books.DTOs;
using YomLog.Shared.Models;

namespace YomLog.MobileApp.Services.Api;

public class GoogleBooksApiService
{
    private readonly HttpClientWrapper _httpClientWrapper;

    public GoogleBooksApiService(HttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Pagination<BookDetailsDTO>> GetBookList(string query, int startIndex, int limit)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new Pagination<BookDetailsDTO>(Enumerable.Empty<BookDetailsDTO>(), 0, startIndex, 1);

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
            return new Pagination<BookDetailsDTO>(Enumerable.Empty<BookDetailsDTO>(), totalItems, startIndex, limit);

        var books = ((IEnumerable<dynamic>)data.Items)
            .Select(x => new BookDetailsDTO
            {
                GoogleBooksId = (string)x.Id,
                Title = (string)x.VolumeInfo.Title,
                Authors = (List<string>?)x.VolumeInfo.Authors ?? new(),
                Description = (string?)x.VolumeInfo.Description,
                GoogleBooksUrl = (string?)x.VolumeInfo.CommandLink,
                ThumbnailUrl = ((string?)(x.VolumeInfo.ImageLinks?.Thumbnail ?? x.VolumeInfo.ImageLinks?.SmallThumbnail))
                    ?.Replace("http://", "https://"),
                TotalPage = new((int?)x.VolumeInfo.PageCount, null),
                Isbn = ((IEnumerable<dynamic>?)x.VolumeInfo.IndustryIdentifiers)?.FirstOrDefault()?.Identifier
            });

        return new Pagination<BookDetailsDTO>(books, totalItems, startIndex, limit);
    }
}