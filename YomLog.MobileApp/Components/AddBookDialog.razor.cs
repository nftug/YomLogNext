using Microsoft.AspNetCore.Components;
using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.MobileApp.Services.Api;

namespace YomLog.MobileApp.Components;

public partial class AddBookDialog : ComponentBase
{
    [Inject] private BookApiService ApiService { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance? MudDialog { get; set; }
    [Parameter] public BookDetailsDTO Item { get; set; } = null!;
    [Parameter] public EventCallback<string> SearchByAuthor { get; set; }

    private BookCommandDTO Command { get; set; } = null!;

    protected override void OnInitialized()
    {
        Command = (BookCommandDTO)Item.ToCommandDTO();
    }

    private async Task AddOrEditBookAsync(DialogBase context)
    {
        var result =
            Item.IsNewItem
            ? await ApiService.AddAsync(Command)
            : await ApiService.EditAsync(Item.Id, Command);
        if (result != null) await context.Ok(result);
    }

    public static async Task<DialogResult> ShowDialog(
        IDialogService dialogService,
        BookDetailsDTO item,
        EventCallback<string> searchByAuthorCallback
    )
    {
        var parameters = new DialogParameters
        {
            ["Item"] = item,
            ["SearchByAuthor"] = searchByAuthorCallback
        };
        var options = new DialogOptions { FullScreen = true };
        var dialog = dialogService.Show<AddBookDialog>("AddBook", parameters, options);
        return await dialog.Result;
    }
}
