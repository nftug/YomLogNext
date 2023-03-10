using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.MobileApp.Services;

public class EnvironmentHelper : IEnvironmentHelper
{
    private readonly IPopupService _popupService;

    public EnvironmentHelper(IPopupService popupService)
    {
        _popupService = popupService;
    }

    public async Task QuitApp()
    {
        bool result = await _popupService.ShowNativeConfirm("確認", "アプリを終了しますか？");
        if (result) Application.Current?.Quit();
    }
}
