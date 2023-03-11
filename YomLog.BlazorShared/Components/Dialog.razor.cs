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
    [Parameter] public bool CloseButton { get; set; } = false;
    [Parameter] public bool CloseByHistoryBack { get; set; } = true;

    public static async Task<DialogResult> ShowDialog(
        IDialogService dialogService,
        string title,
        string contentText,
        string okText = "OK",
        string? cancelText = "Cancel",
        Color okColor = Color.Primary,
        Variant okVariant = Variant.Text,
        MaxWidth maxWidth = MaxWidth.Small,
        bool closeButton = false,
        bool closeByHistoryBack = true
    )
    {
        var parameters = new DialogParameters
        {
            [nameof(ContentText)] = contentText,
            [nameof(OkText)] = okText,
            [nameof(CancelText)] = cancelText,
            [nameof(OkColor)] = okColor,
            [nameof(OkVariant)] = okVariant,
            [nameof(CloseButton)] = closeButton,
            [nameof(CloseByHistoryBack)] = closeByHistoryBack
        };
        var options = new DialogOptions { MaxWidth = maxWidth };
        var dialog = dialogService.Show<Dialog>(title, parameters, options);
        return await dialog.Result;
    }
}