using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services;
using YomLog.MobileApp.Components;
using YomLog.MobileApp.Entities;
using YomLog.MobileApp.Services.Api;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Pages;

public partial class AddBookPage : BindableComponentBase
{
    [Inject] private GoogleBooksApiService ApiService { get; set; } = null!;
    [Inject] private LoggerService LoggerService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }

    private int _totalItems;
    private List<BookInfo> _results = new();
    private int _startIndex;
    private bool _reachedLast;
    private AppBarSearchBar? _searchbar;

    private async Task SearchAsync()
    {
        _totalItems = 0;
        _startIndex = 0;
        _results = new();
        _reachedLast = false;

        await GetBooksAsync();
        StateHasChanged();
    }

    private void SearchByAuthor(string author)
    {
        _searchbar!.NavigateForSearch($"inauthor:\"{author}\"");
    }

    private async Task GetBooksAsync()
    {
        if (string.IsNullOrEmpty(Query)) return;

        int count = 12;
        if (_totalItems == 0)
        {
            var paginated = await ApiService.GetBookList(Query, 0, 1);
            _totalItems = paginated.TotalItems;
            await Task.Delay(500);
        }

        int numItems = Math.Min(count, _totalItems - _startIndex);

        try
        {
            await LoggerService.Print($"StartIndex: {_startIndex}, NumItems: {numItems}");

            var data = await ApiService.GetBookList(Query, _startIndex, numItems);
            _results.AddRange(data.Results);
            _startIndex += data.Results.Count();

            if (_totalItems <= _startIndex)
            {
                _reachedLast = true;
                return;
            }
        }
        catch (Exception e) when (e is IApiException exception)
        {
            await LoggerService.Print(exception.Message!);
        }
    }
}
