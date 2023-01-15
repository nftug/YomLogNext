using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services;
using YomLog.MobileApp.Entities;
using YomLog.MobileApp.Services.Api;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Pages;

public partial class AddBookPage : BindableComponentBase
{
    [Inject] private GoogleBooksApiService ApiService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private LoggerService LoggerService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }

    private ReactivePropertySlim<string?> SearchQuery { get; set; } = null!;
    private int _totalItems;
    private List<BookInfo> _results = new();
    private int _startIndex;
    private bool _reachedLast;

    protected override void OnInitialized()
    {
        SearchQuery = new ReactivePropertySlim<string?>().AddTo(Disposable);
        SearchQuery.Skip(1).Subscribe(async _ => await ClearList());
    }

    protected override void OnParametersSet()
    {
        if (SearchQuery.Value != Query)
            SearchQuery.Value = Query;
    }

    private async Task ClearList()
    {
        _totalItems = 0;
        _startIndex = 0;
        _results = new();
        _reachedLast = false;

        var uri = NavigationManager.GetUriWithQueryParameter(nameof(Query), SearchQuery.Value);
        NavigationManager.NavigateTo(uri);

        await GetBooksAsync();
        StateHasChanged();
    }

    private void SearchByAuthor(string author)
    {
        SearchQuery.Value = $"inauthor:\"{author}\"";
    }

    private async Task GetBooksAsync()
    {
        await LoggerService.Print($"SearchQuery.Value = {SearchQuery.Value}");

        if (string.IsNullOrEmpty(SearchQuery.Value)) return;

        int count = 12;
        if (_totalItems == 0)
        {
            var paginated = await ApiService.GetBookList(SearchQuery.Value, 0, 1);
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
            var data = await ApiService.GetBookList(SearchQuery.Value, _startIndex, numItems);
            _results.AddRange(data.Results);
            _results = _results.DistinctBy(x => x.Id).ToList();
            _startIndex += numItems;
        }
        catch (Exception e) when (e is IApiException)
        {
        }
    }
}
