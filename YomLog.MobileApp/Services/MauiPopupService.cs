using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.MobileApp.Services;

public class MauiPopupService : IPopupService
{
    private readonly IDialogService _dialogService;

    public MauiPopupService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> ShowConfirm(string title, string message, string okText = "OK", string cancelText = "Cancel")
    {
        var dialog = await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText);
        return !dialog.Canceled;
    }

    public async Task ShowPopup(string title, string message, string okText = "OK")
        => await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText: null);

    public async Task ShowNativePopup(string title, string message, string okText = "OK")
        => await Application.Current!.MainPage!.DisplayAlert(title, message, okText);

    public async Task<bool> ShowNativeConfirm
        (string title, string message, string okText = "OK", string cancelText = "Cancel")
        => await Application.Current!.MainPage!.DisplayAlert(title, message, okText, cancelText);

    public async Task<string> ShowNativePrompt
        (string title, string message, string okText = "OK", string cancelText = "Cancel")
        => await Application.Current!.MainPage!.DisplayPromptAsync(title, message, okText, cancelText);

    /*
    public async Task<string?> ShowPrompt(
        string title,
        string message,
        string okText = "OK",
        string cancelText = "Cancel",
        string? placeHolder = null
    )
        => await Application.Current!.MainPage!.DisplayPromptAsync(title, message, okText, cancelText, placeHolder);
    */
}
