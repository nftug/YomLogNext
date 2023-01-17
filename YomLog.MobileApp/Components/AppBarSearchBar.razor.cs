using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace YomLog.MobileApp.Components;

public partial class AppBarSearchBar : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter, EditorRequired] public string? Query { get; set; }
    [Parameter, EditorRequired] public EventCallback<string?> NavigateAction { get; set; }
    [Parameter, EditorRequired] public EventCallback SearchAction { get; set; }
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;

    private bool _showingSearchBar;
    private string? _searchFieldText;
    private MudTextField<string?>? _searchField;
    private string? _previousQuery;

    private async Task OnSubmitSearchForm()
    {
        if (!string.IsNullOrEmpty(_searchFieldText))
            await NavigateAction.InvokeAsync(_searchFieldText);
    }

    protected override async Task OnParametersSetAsync()
    {
        // Debug.WriteLine($"OnParameters: Query = {Query}, _previousQuery = {_previousQuery}");

        if (Query != _previousQuery)
        {
            await RecoverSearchBarState();
            _searchFieldText = Query;
            await InvokeAsync(StateHasChanged);

            if (!string.IsNullOrEmpty(Query))
                await SearchAction.InvokeAsync();
        }

        _previousQuery = Query;
    }

    private async Task RecoverSearchBarState()
    {
        string activeTagName = await JSRuntime.InvokeAsync<string>("getActiveElementTagName");
        if (activeTagName != "BODY") return;

        if (_searchField != null) await _searchField.BlurAsync();
        _showingSearchBar = !string.IsNullOrEmpty(Query);
    }
}
