using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Popup;
using YomLog.Domain.Books.DTOs;
using YomLog.MobileApp.Components;
using YomLog.MobileApp.Services.Api;
using YomLog.MobileApp.Services.Stores;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Pages;

public partial class BookDetailsPage : BindableComponentBase
{
    [Inject] private BookStoreService BookStore { get; set; } = null!;
    [Inject] private ProgressApiService ProgressApiService { get; set; } = null!;
    [Inject] private BookApiService BookApiService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IPopupService PopupService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter] public Guid BookId { get; set; }

    private ReactivePropertySlim<bool> IsLoading = null!;
    private BookDetailsDTO Book = null!;
    private List<ProgressDetailsDTO> ProgressList = new();

    private ProgressDetailsDTO CurrentProgress
        => Book.CurrentProgress ?? new() { BookId = Book.Id };

    protected override void OnInitialized()
    {
        IsLoading = new ReactivePropertySlim<bool>(true).AddTo(Disposable);
        IsLoading.Skip(1).Subscribe(_ => Rerender());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsLoading.Value = true;
        Book = BookStore.GetOrDefault(BookId) ?? throw new NotFoundException();
        ProgressList = await ProgressApiService.FetchByBookAsync(BookId);
        IsLoading.Value = false;
    }

    private async Task EditMenuClicked()
    {
        var dialog = await BookEditDialog.ShowDialog(DialogService, Book, default);
        if (dialog.Data is bool ans && ans) await LoadDataAsync();
    }

    private async Task DeleteMenuClicked()
    {
        bool confirm = await PopupService.ShowConfirm("確認", "この本を削除しますか？");
        if (!confirm) return;

        await BookApiService.DeleteAsync(Book);
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async Task AddProgressClicked()
    {
        var dialog = await ProgressEditDialog.ShowDialog(DialogService, null, Book);
        if (dialog.Data is bool ans && ans) await LoadDataAsync();
    }
}
