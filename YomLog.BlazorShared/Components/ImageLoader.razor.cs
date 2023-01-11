using YomLog.BlazorShared.Extensions;
using YomLog.Shared.ValueObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Reactive.Bindings;
using YomLog.BlazorShared.Models;
using Reactive.Bindings.Extensions;

namespace YomLog.BlazorShared.Components;

public partial class ImageLoader : BindableComponentBase
{
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public string? Source { get; set; }
    [Parameter] public PhotoDTO Photo { get; set; } = null!;
    [Parameter] public EventCallback<string> SourceChanged { get; set; }
    [Parameter] public EventCallback<PhotoDTO> PhotoChanged { get; set; }

    private readonly int MaxFileSize = 10 * 1024 * 1024;

    private string? _contentType;
    private string? ImageSrc
        => Source != null
            ? $"data:{_contentType};base64,{Source}"
            : Photo.Id.HasValue
            ? Photo.Uri
            : null;
    private PhotoDTO _photoOrigin = null!;

    private ReactivePropertySlim<bool> _isLoading = new();

    protected override void OnInitialized()
    {
        _photoOrigin = Photo;
        _isLoading.AddTo(Disposable);
    }

    private async Task OnSelectImage(InputFileChangeEventArgs e)
    {
        if (!e.File.ContentType.StartsWith("image")) return;

        Source = null;
        Photo = new();
        await PhotoChanged.InvokeAsync(Photo);

        _isLoading.Value = true;
        try
        {
            using var stream = e.File.OpenReadStream(MaxFileSize);

            Source = await stream.ConvertToBase64StringAsync();
            await SourceChanged.InvokeAsync(Source);
            _contentType = e.File.ContentType;
        }
        catch (IOException)
        {
            Snackbar.Add("File size is too large!", Severity.Error);
        }
        finally
        {
            _isLoading.Value = false;
        }
    }

    private async void OnDeleteImage()
    {
        Source = null;
        Photo = new();
        await SourceChanged.InvokeAsync(null);
        await PhotoChanged.InvokeAsync(Photo);
    }

    private async void OnResetImage()
    {
        Source = null;
        Photo = _photoOrigin;
        await SourceChanged.InvokeAsync(null);
        await PhotoChanged.InvokeAsync(Photo);
    }
}
