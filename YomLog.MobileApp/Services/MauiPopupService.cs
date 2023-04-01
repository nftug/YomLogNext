using Microsoft.JSInterop;
using MudBlazor;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.MobileApp.Services;

public class MauiPopupService : BlazorPopupService
{
    public MauiPopupService(IDialogService dialogService, IJSRuntime jsRuntime) : base(dialogService, jsRuntime)
    {
    }

    public override Task ShowNativePopup(string title, string message, string okText = "OK")
        => Application.Current!.MainPage!.DisplayAlert(title, message, okText);

    public override Task<bool> ShowNativeConfirm(string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => Application.Current!.MainPage!.DisplayAlert(title, message, okText, cancelText);

    public override Task<string> ShowNativePrompt(string title, string message, string okText = "OK", string cancelText = "キャンセル")
        => Application.Current!.MainPage!.DisplayPromptAsync(title, message, okText, cancelText);
}
