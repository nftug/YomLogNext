namespace YomLog.MobileApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        if (window != null)
        {
            window.Title = AppInfo.Name;
            window.Destroying += OnDestroying;
        }

        return window!;
    }

    private void OnDestroying(object? sender, EventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            Current?.Quit();
    }
}
