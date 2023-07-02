using Microsoft.AspNetCore.Components;
using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.MobileApp.Services.Api;

namespace YomLog.MobileApp.Components;

public partial class ProgressEditDialog : ComponentBase
{
    [Inject] private ProgressApiService ApiService { get; set; } = null!;

    [Parameter] public ProgressDetailsDTO Item { get; set; } = null!;
    [Parameter] public BookDetailsDTO Book { get; set; } = null!;

    private ProgressCommandDTO Command = null!;

    protected override void OnInitialized()
    {
        Item ??= new()
        {
            BookId = Book.Id,
            Position = Book.CurrentProgress?.Position ?? new()
        };
        Command = (ProgressCommandDTO)Item.ToCommandDTO();
    }

    private async Task AddOrEditProgressAsync(DialogBase context)
    {
        var result =
            Item.IsNewItem
            ? await ApiService.AddAsync(Command)
            : await ApiService.EditAsync(Item.Id, Command);
        if (result != null) await context.Ok(true);
    }

    public static async Task<DialogResult> ShowDialog(
        IDialogService dialogService,
        ProgressDetailsDTO? item,
        BookDetailsDTO book
    )
    {
        var parameters = new DialogParameters
        {
            [nameof(Item)] = item,
            [nameof(Book)] = book
        };
        var dialog = dialogService.Show<ProgressEditDialog>("", parameters);
        return await dialog.Result;
    }
}
