using Microsoft.AspNetCore.Components;
using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Services;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.MobileApp.Components;

public partial class AddBookDialog : ComponentBase
{
    [Inject] private BookService BookService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance? MudDialog { get; set; }
    [Parameter] public BookCommandDTO Item { get; set; } = null!;
    [Parameter] public EventCallback<string> SearchByAuthor { get; set; }

    private async Task AddBookAsync(DialogBase context)
    {
        try
        {
            var book = await BookService.CreateNewBook(Item, User.GetDummyUser());
            Snackbar.Add("本を追加しました。", Severity.Info);
            await context.Ok(book);
        }
        catch (EntityValidationException e)
        {
            Snackbar.Add(e.Errors.First().Value.First(), Severity.Error);
        }
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
