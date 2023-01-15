using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YomLog.BlazorShared.Services;

namespace YomLog.MobileApp.Components;

public partial class AppBarSearchBar : ComponentBase, IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private PageInfoService PageInfoService { get; set; } = null!;

    [Parameter, EditorRequired] public string? Query { get; set; }
    [Parameter, EditorRequired] public EventCallback SearchAction { get; set; }
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;

    private bool _showingSearchBar;
    private string? _searchText;
    private MudTextField<string?>? _searchField;

    protected override void OnInitialized()
    {
        PageInfoService.PathChanged += OnPathChanged;
    }

    public void Dispose()
    {
        PageInfoService.PathChanged -= OnPathChanged;
        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        if (_searchText != Query) _searchText = Query;
    }

    public void NavigateForSearch(string? searchText)
    {
        var uri = NavigationManager.GetUriWithQueryParameter(nameof(Query), searchText);
        NavigationManager.NavigateTo(uri);
    }

    private async void OnPathChanged(string previous, string current)
    {
        if (_searchField != null) await _searchField.BlurAsync();
        await SearchAction.InvokeAsync();
    }

    private async Task RecoverSearchBarState()
    {
        string activeTagName = await JSRuntime.InvokeAsync<string>("getActiveElementTagName");
        if (activeTagName != "BODY") return;

        _showingSearchBar = !string.IsNullOrEmpty(Query);
        LayoutService.RequestAppBarRerender();
    }
}
