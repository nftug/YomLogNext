using Dynamic.Json;
using Microsoft.AspNetCore.WebUtilities;
using YomLog.Domain.Books.DTOs;
using YomLog.Shared.Attributes;
using YomLog.Shared.Extensions;
using YomLog.Shared.Models;

namespace YomLog.MobileApp.Services.Api;

[InjectAsScoped]
public class GoogleBooksApiService
{
    private readonly HttpClient _httpClient;

    public GoogleBooksApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

        var response = await _httpClient.GetAsync(url);
        var rawContent = await response.Content.ReadAsStringAsync();
        var data = DJson.Parse(rawContent);

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
                TotalPage = new(((int?)x.VolumeInfo.PageCount) ?? 0, null, skipValidation: true),
                Isbn = ((IEnumerable<dynamic>?)x.VolumeInfo.IndustryIdentifiers)?.FirstOrDefault()?.Identifier
            })
            .ToList();

        return new Pagination<BookDetailsDTO>(books, totalItems, startIndex, limit);
    }
}