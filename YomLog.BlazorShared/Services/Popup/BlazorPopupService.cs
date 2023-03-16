using YomLog.BlazorShared.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services.Popup;

[InjectAsScoped]
public class BlazorPopupService : IPopupService
{
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;

    public BlazorPopupService(IDialogService dialogService, IJSRuntime jsRuntime)
    {
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> ShowConfirm(string title, string message, string okText = "OK", string cancelText = "キャンセル")
    {
        var dialog = await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText);
        return !dialog.Canceled;
    }

    public Task ShowImagePopup(string? uri)
        => ImageDialog.ShowDialog(_dialogService, uri);

    public Task ShowPopup(string title, string message, string okText = "OK")
        => Dialog.ShowDialog(_dialogService, title, message, okText, cancelText: null);

    public Task ShowNativePopup(string title, string message, string okText = "OK")
        => _jsRuntime.InvokeVoidAsync("alert", message).AsTask();

    public Task<bool> ShowNativeConfirm
        (string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => _jsRuntime.InvokeAsync<bool>("confirm", message).AsTask();

    public Task<string> ShowNativePrompt
        (string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => _jsRuntime.InvokeAsync<string>("prompt", message).AsTask();
}
