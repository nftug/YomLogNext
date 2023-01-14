using Microsoft.AspNetCore.Components;
using YomLog.MobileApp.Entities;
using YomLog.MobileApp.Services.Api;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Pages;

public partial class AddBookPage : ComponentBase
{
    [Inject] private GoogleBooksApiService ApiService { get; set; } = null!;

    private string _searchQuery = string.Empty;
    private int _totalItems;
    private List<BookInfo> _results = new();
    private int _startIndex;
    private bool _reachedLast;

    private async Task ClearList()
    {
        _totalItems = 0;
        _startIndex = 0;
        _results = new();
        _reachedLast = false;
        await GetBooksAsync();
        StateHasChanged();
    }

    private async Task SearchByAuthor(string author)
    {
        _searchQuery = $"inauthor:\"{author}\"";
        await ClearList();
    }

    private async Task GetBooksAsync()
    {
        int count = 12;
        if (_totalItems <= 0)
        {
            var paginated = await ApiService.GetBookList(_searchQuery, 0, 1);
            _totalItems = paginated.TotalItems;
        }

        var numItems = Math.Min(count, _totalItems - _startIndex);
        if (_results.Count >= _startIndex + numItems)
        {
            _reachedLast = true;
            return;
        }

        try
        {
            var data = await ApiService.GetBookList(_searchQuery, _startIndex, numItems);
            _results.AddRange(data.Results);
            _results = _results.DistinctBy(x => x.Id).ToList();
            _startIndex += numItems;
        }
        catch (Exception e) when (e is IApiException)
        {
        }
    }
}
