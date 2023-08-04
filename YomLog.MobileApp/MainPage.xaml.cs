namespace YomLog.MobileApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    void MainPage_Loaded(object? sender, EventArgs e)
    {
#if MACCATALYST && DEBUG
        var view = (WebKit.WKWebView)blazorWebView.Handler!.PlatformView!;
        view.SetValueForKey(Foundation.NSObject.FromObject(true), new Foundation.NSString("inspectable"));
#endif
    }
}
