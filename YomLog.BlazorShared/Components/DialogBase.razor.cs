using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;

namespace YomLog.BlazorShared.Components;

public partial class DialogBase : BindableComponentBase
{
    [CascadingParameter] protected MudDialogInstance? MudDialog { get; set; }
    [Parameter] public RenderFragment<DialogBase>? TitleContent { get; set; } = null!;
    [Parameter] public RenderFragment<DialogBase> DialogContent { get; set; } = null!;
    [Parameter] public RenderFragment<DialogBase>? DialogActions { get; set; } = null!;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool DisableSidePadding { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public bool CloseButton { get; set; }
    [Parameter] public bool RenderDialog { get; set; } = true;
    [Parameter] public EventCallback<DialogCloseContext> OnBeforeClose { get; set; }

    private object? Result { get; set; }
    public MudDialog? Dialog { get; set; }
    public static readonly string ShouldForceNavigate = nameof(ShouldForceNavigate);

    protected override void OnInitialized()
    {
        LayoutService.IsProcessing.Subscribe(_ => Rerender()).AddTo(Disposable);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || MudDialog == null) return;
        await JSRuntime.InvokeVoidAsync("disableScroll");
    }

    public Task Ok<T>(T result) => OnCloseDialog(result);

    public Task Cancel() => OnCloseDialog(null);

    private async Task OnCloseDialog(object? result)
    {
        Result = result;
        await CloseDialog();
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (context.HistoryEntryState != ShouldForceNavigate && !context.IsNavigationIntercepted)
            context.PreventNavigation();

        await CloseDialog();
    }

    private async Task CloseDialog()
    {
        await JSRuntime.InvokeVoidAsync("enableScroll");
        if (Result != null)
        {
            MudDialog?.Close(DialogResult.Ok(Result));
        }
        else
        {
            if (OnBeforeClose.HasDelegate)
            {
                var context = new DialogCloseContext();
                await OnBeforeClose.InvokeAsync(context);
                if (context.IsClosePrevented) return;
            }

            MudDialog?.Cancel();
        }
    }
}

public class DialogCloseContext
{
    internal bool IsClosePrevented { get; private set; }
    public void PreventClose() => IsClosePrevented = true;
}