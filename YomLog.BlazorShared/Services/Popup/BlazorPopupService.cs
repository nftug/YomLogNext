using YomLog.BlazorShared.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace YomLog.BlazorShared.Services.Popup;

public class BlazorPopupService : IPopupService
{
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _jsRuntime;

    public BlazorPopupService(IDialogService dialogService, IJSRuntime jsRuntime)
    {
        _dialogService = dialogService;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> ShowConfirm(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var dialog = await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText);
        return !dialog.Canceled;
    }

    public async Task ShowPopup(string title, string message, string okText = "OK")
        => await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText: null);

    public async Task ShowNativePopup(string title, string message, string okText = "OK")
        => await _jsRuntime.InvokeVoidAsync("alert", message);

    public async Task<bool> ShowNativeConfirm
        (string title, string message, string okText = "OK", string cancelText = "Cancel")
        => await _jsRuntime.InvokeAsync<bool>("confirm", message);

    public async Task<string> ShowNativePrompt
        (string title, string message, string okText = "OK", string cancelText = "Cancel")
        => await _jsRuntime.InvokeAsync<string>("prompt", message);
}
