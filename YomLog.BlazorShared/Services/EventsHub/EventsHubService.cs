using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Auth;

namespace YomLog.BlazorShared.Services.EventsHub;

public class EventsHubService : BindBase, IEventsHubService
{
    protected readonly IAuthService _authService;
    protected readonly HttpClientWrapper _httpClientWrapper;
    protected readonly LayoutService _layoutService;

    public EventsHubService(
        LayoutService layoutService,
        IAuthService authService,
        HttpClientWrapper httpClientWrapper
    )
    {
        _layoutService = layoutService;
        _authService = authService;
        _httpClientWrapper = httpClientWrapper;

        // NOTE:
        // To add necessary events, edit the following parts:

        // NOTE: Do not insert Skip(1) when subscribing authentication state.
        // Due to some bugs in Blazor MAUI client, skipping once prevents from first-time firing of events.
        _authService.IsAuthenticated
            .Where(v => v)
            .Subscribe(_ => Handle(OnAuthenticated))
            .AddTo(Disposable);

        _authService.IsAuthenticated
            .Skip(1)
            .Where(v => !v)
            .Subscribe(_ => Handle(OnLogout))
            .AddTo(Disposable);

        _httpClientWrapper.IsOffline
            .Skip(1)
            .Subscribe(v => Handle(() => OnChangeIsOffline(v)))
            .AddTo(Disposable);
    }

    // NOTE:
    // The following virtual methods are implemented only for a delivered class.
    // Do not edit them directly!
    protected virtual void OnAuthenticated() { }

    protected virtual void OnLogout() { }

    protected virtual void OnChangeIsOffline(bool isOffline) { }

    protected void Handle(Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            _layoutService.Exception.Value = e;
        }
    }
}
