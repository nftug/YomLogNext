using MudBlazor;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Services.Popup;
using YomLog.Shared.Attributes;

namespace YomLog.MobileApp.Services;

[InjectAsScoped]
public class MauiPopupService : IPopupService
{
    private readonly IDialogService _dialogService;

    public MauiPopupService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> ShowConfirm(string title, string message, string okText = "OK", string cancelText = "キャンセル")
    {
        var dialog = await Dialog.ShowDialog(_dialogService, title, message, okText, cancelText);
        return !dialog.Canceled;
    }

    public Task ShowPopup(string title, string message, string okText = "OK")
        => Dialog.ShowDialog(_dialogService, title, message, okText, cancelText: null);

    public Task ShowImagePopup(string? uri)
        => ImageDialog.ShowDialog(_dialogService, uri);

    public Task ShowNativePopup(string title, string message, string okText = "OK")
        => Application.Current!.MainPage!.DisplayAlert(title, message, okText);

    public Task<bool> ShowNativeConfirm
        (string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => Application.Current!.MainPage!.DisplayAlert(title, message, okText, cancelText);

    public Task<string> ShowNativePrompt
        (string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => Application.Current!.MainPage!.DisplayPromptAsync(title, message, okText, cancelText);

    /*
    public async Task<string?> ShowPrompt(
        string title,
        string message,
        string okText = "OK",
        string cancelText = "キャンセル",
        string? placeHolder = null
    )
        => await Application.Current!.MainPage!.DisplayPromptAsync(title, message, okText, cancelText, placeHolder);
    */
}
