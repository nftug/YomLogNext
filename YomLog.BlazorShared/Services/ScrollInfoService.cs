using Microsoft.JSInterop;
using MudBlazor;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
public class ScrollInfoService
{
    public event EventHandler<double>? OnScroll;

    private readonly PageInfoService _pageInfoService;
    private readonly IJSRuntime _jsRuntime;
    private readonly IScrollManager _scrollManager;
    private readonly IScrollListenerFactory _scrollListenerFactory;

    private IScrollListener _scrollListener = null!;

    public ScrollInfoService(
        PageInfoService pageInfoService,
        IJSRuntime jsRuntime,
        IScrollManager scrollManager,
        IScrollListenerFactory scrollListenerFactory
    )
    {
        _pageInfoService = pageInfoService;
        _jsRuntime = jsRuntime;
        _scrollManager = scrollManager;
        _scrollListenerFactory = scrollListenerFactory;
    }

    public async Task RegisterService()
    {
        await _jsRuntime.InvokeVoidAsync("registerScrollInfoService", DotNetObjectReference.Create(this));
        _scrollListener = _scrollListenerFactory.Create(null);
    }

    private readonly Dictionary<string, double> _scrollYPerPage = new();
    public double? ScrollY
    {
        get => _scrollYPerPage.TryGetValue(_pageInfoService.PathAndQuery.Value, out double value) ? value : null;
        private set
        {
            if (value == null) return;
            _scrollYPerPage[_pageInfoService.PathAndQuery.Value] = (double)value;
        }
    }

    public async Task ScrollToTopAsync()
        => await _scrollManager.ScrollToTopAsync(_scrollListener.Selector, ScrollBehavior.Smooth);

    public async void ResetScroll()
    {
        ScrollY = 0;
        await _jsRuntime.InvokeVoidAsync("setScrollY", 0);
    }

    [JSInvokable("OnScroll")]
    public void SetScrollY(double scrollY)
    {
        ScrollY = scrollY;
        OnScroll?.Invoke(this, scrollY);
    }

    private bool _beingResumed;

    [JSInvokable("OnPopState")]
    public async Task RestoreScroll()
    {
        if (ScrollY == null || _beingResumed) return;

        _beingResumed = true;
        // await Task.Delay(200);
        await _jsRuntime.InvokeVoidAsync("setScrollY", ScrollY);
        _beingResumed = false;
    }
}
