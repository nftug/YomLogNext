using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace YomLog.BlazorShared.Components;

public partial class AppBarSearchField : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter, EditorRequired] public string? Query { get; set; }
    [Parameter, EditorRequired] public EventCallback<string?> NavigateAction { get; set; }
    [Parameter, EditorRequired] public EventCallback SearchAction { get; set; }
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public bool AutoFocus { get; set; } = true;

    private bool _showingSearchField;
    private string? _searchFieldText;
    private MudTextField<string?>? _searchField;
    private string? _previousQuery;

    private bool IsQueryEmpty => string.IsNullOrEmpty(Query);

    private async Task OnSubmitSearchForm()
    {
        if (!string.IsNullOrEmpty(_searchFieldText))
            await NavigateAction.InvokeAsync(_searchFieldText);
    }

    protected override async Task OnParametersSetAsync()
    {
        // DebugLogger.Print($"IsQueryEmpty = {IsQueryEmpty}, AutoFocus = {AutoFocus}");

        if (IsQueryEmpty && AutoFocus)
        {
            _showingSearchField = true;
            // DebugLogger.Print("Focus");
        }
        else
        {
            await RecoverSearchFieldState();
            // DebugLogger.Print("Blur");
        }

        if (Query != _previousQuery)
        {
            _searchFieldText = Query;
            if (!IsQueryEmpty) await SearchAction.InvokeAsync();
        }

        _previousQuery = Query;
    }

    private async Task RecoverSearchFieldState(bool checkFocus = false)
    {
        if (checkFocus)
        {
            string activeTagName = await JSRuntime.InvokeAsync<string>("getActiveElementTagName");
            if (activeTagName != "BODY") return;
        }

        _showingSearchField = !IsQueryEmpty || AutoFocus;

        if (_searchField != null && !IsQueryEmpty)
        {
            await _searchField.BlurAsync();
        }
    }
}
