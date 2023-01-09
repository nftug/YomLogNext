using Microsoft.AspNetCore.Components;
using MudBlazor;
using Color = MudBlazor.Color;

namespace YomLog.BlazorShared.Components;

public partial class Dialog : ComponentBase
{
    [CascadingParameter] protected MudDialogInstance? MudDialog { get; set; }
    [Parameter] public string ContentText { get; set; } = string.Empty;
    [Parameter] public string? OkText { get; set; }
    [Parameter] public string? CancelText { get; set; }
    [Parameter] public Color OkColor { get; set; } = Color.Primary;
    [Parameter] public Variant OkVariant { get; set; } = Variant.Text;

    public static async Task<DialogResult> ShowDialog(
        IDialogService dialogService,
        string title,
        string contentText,
        string okText = "OK",
        string? cancelText = "Cancel",
        Color okColor = Color.Primary,
        Variant okVariant = Variant.Text,
        MaxWidth maxWidth = MaxWidth.Small
    )
    {
        var parameters = new DialogParameters
        {
            ["ContentText"] = contentText,
            ["OkText"] = okText,
            ["CancelText"] = cancelText,
            ["OkColor"] = okColor,
            ["OkVariant"] = okVariant
        };
        var options = new DialogOptions { MaxWidth = maxWidth };
        var dialog = dialogService.Show<Dialog>(title, parameters, options);
        var result = await dialog.Result;

        // Wait for window.showingDialog changed into false
        await Task.Delay(200);
        return result;
    }
}