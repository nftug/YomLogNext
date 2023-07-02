using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Domain.Books.DTOs;
using YomLog.MobileApp.Services.Api;

namespace YomLog.MobileApp.Pages;

public partial class BookAddPage : BindableComponentBase
{
    [Inject] private GoogleBooksApiService ApiService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }

    private List<BookDetailsDTO> _results = new();
    private int _startIndex;
    private bool _reachedLast;
    private ReactivePropertySlim<bool> IsLoading { get; set; } = null!;
    private readonly int Limit = 12;

    protected override void OnInitialized()
    {
        IsLoading = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsLoading.Skip(1).Subscribe(_ => Rerender());
    }

    private async Task SearchAsync()
    {
        _startIndex = 0;
        _results = new();
        await GetBooksAsync();
    }

    private void SearchByAuthor(string author) => OnNavigateForSearch($@"inauthor:""{author}""");

    private void OnNavigateForSearch(string query)
        => NavigationManager.NavigateTo(
                NavigationManager.GetUriWithQueryParameter(nameof(Query), query)
            );

    private async Task GetBooksAsync()
    {
        if (string.IsNullOrEmpty(Query) || IsLoading.Value) return;
        if (_reachedLast) return;

        try
        {
            IsLoading.Value = true;

            var data = await ApiService.GetBookList(Query, _startIndex, Limit);
            _results.AddRange(data.Results);
            _startIndex += data.Results.Count();
            _reachedLast = !data.Results.Any();
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
        finally
        {
            IsLoading.Value = false;
        }
    }
}
