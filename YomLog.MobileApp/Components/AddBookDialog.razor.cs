using Microsoft.AspNetCore.Components;
using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.Domain.Books.Commands;
using YomLog.MobileApp.Services.Api;

namespace YomLog.MobileApp.Components;

public partial class AddBookDialog : ComponentBase
{
    [Inject] private BookApiService ApiService { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance? MudDialog { get; set; }
    [Parameter] public BookCommandDTO Item { get; set; } = null!;
    [Parameter] public EventCallback<string> SearchByAuthor { get; set; }

    private async Task AddBookAsync(DialogBase context)
    {
        var result = await ApiService.AddAsync(Item);
        if (result != null) await context.Ok(result);
    }

    public static async Task<DialogResult> ShowDialog(
        IDialogService dialogService,
        BookCommandDTO item,
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
