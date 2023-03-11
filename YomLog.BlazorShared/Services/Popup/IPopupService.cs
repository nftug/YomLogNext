namespace YomLog.BlazorShared.Services.Popup;

public interface IPopupService
{
    Task ShowPopup(string title, string message, string okText = "OK");
    Task<bool> ShowConfirm
        (string title, string message, string okText = "OK", string cancelText = "キャンセル");
    Task ShowImagePopup(string? uri);
    /*
    public Task<string?> ShowPrompt(
        string title,
        string message,
        string okText = "OK",
        string cancelText = "キャンセル",
        string? placeHolder = null
    );
    */

    Task ShowNativePopup(string title, string message, string okText = "OK");
    Task<bool> ShowNativeConfirm
        (string title, string message, string okText = "OK", string cancelText = "キャンセル");
    Task<string> ShowNativePrompt(string title, string message, string okText = "OK", string cancelText = "キャンセル");
}
