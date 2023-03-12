using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Models;
using YomLog.Domain.Books.Commands;
using YomLog.MobileApp.Services.Api;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Pages;

public partial class AddBookPage : BindableComponentBase
{
    [Inject] private GoogleBooksApiService ApiService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }

    private int _totalItems;
    private List<BookCommandDTO> _results = new();
    private int _startIndex;
    private bool ReachedLast => _totalItems <= _startIndex;
    private ReactivePropertySlim<bool> IsLoading { get; set; } = null!;
    private readonly int Limit = 12;

    protected override void OnInitialized()
    {
        IsLoading = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsLoading.Subscribe(_ => Rerender());
    }

    private async Task SearchAsync()
    {
        _totalItems = 0;
        _startIndex = 0;
        _results = new();

        await GetBooksAsync();
    }

    private void SearchByAuthor(string author)
    {
        OnNavigateForSearch($@"inauthor:""{author}""");
    }

    private void OnNavigateForSearch(string query)
    {
        var uri = NavigationManager.GetUriWithQueryParameter(nameof(Query), query);
        NavigationManager.NavigateTo(uri, new NavigationOptions
        {
            HistoryEntryState = DialogBase.ShouldForceNavigate
        });
    }

    private async Task GetBooksAsync()
    {
        if (string.IsNullOrEmpty(Query) || IsLoading.Value) return;

        try
        {
            IsLoading.Value = true;

            if (_totalItems == 0)
            {
                var paginated = await ApiService.GetBookList(Query, 0, 1);
                _totalItems = paginated.TotalItems;
                await Task.Delay(500);
            }

            int numItems = Math.Min(Limit, _totalItems - _startIndex);

            var data = await ApiService.GetBookList(Query, _startIndex, numItems);
            _results.AddRange(data.Results);
            _startIndex += data.Results.Count();
        }
        catch (Exception e) when (e is IApiException exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.Message!);
        }
        finally
        {
            IsLoading.Value = false;
        }
    }
}
