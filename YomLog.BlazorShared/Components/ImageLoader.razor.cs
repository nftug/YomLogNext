using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Reactive.Bindings;
using YomLog.BlazorShared.Models;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Net.Mime;
using YomLog.BlazorShared.Services.Api;

namespace YomLog.BlazorShared.Components;

public partial class ImageLoader : BindableComponentBase
{
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private PictureApiService PictureApiService { get; set; } = null!;

    [Parameter] public Guid PictureId { get; set; }
    [Parameter] public EventCallback<Guid> PictureIdChanged { get; set; }
    [Parameter] public IBrowserFile? File { get; set; }
    [Parameter] public EventCallback<IBrowserFile> FileChanged { get; set; }
    [Parameter] public string ContentType { get; set; } = MediaTypeNames.Image.Jpeg;

    private readonly long MaxFileSize = long.MaxValue;
    private string? _imageSrc;
    private Guid _pictureIdOrigin;
    private ReactivePropertySlim<bool> IsLoading { get; set; } = null!;

    protected override void OnInitialized()
    {
        IsLoading = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsLoading.Skip(1).Subscribe(_ => Rerender());

        _pictureIdOrigin = PictureId;
        if (PictureId != default)
            _imageSrc = PictureApiService.GetPicturePath(PictureId);
    }

    private async Task OnSelectImage(InputFileChangeEventArgs e)
        => await SetPicture(Guid.NewGuid(), e.File);

    private async void OnDeleteImage()
        => await SetPicture(default, null);

    private async void OnResetImage()
        => await SetPicture(_pictureIdOrigin, null);

    private async Task SetPicture(Guid pictureId, IBrowserFile? file)
    {
        PictureId = pictureId;
        await PictureIdChanged.InvokeAsync(PictureId);
        File = file;
        await FileChanged.InvokeAsync(File);

        if (File is null)
        {
            _imageSrc = PictureId != default ? PictureApiService.GetPicturePath(PictureId) : null;
            return;
        }

        try
        {
            if (File.ContentType != ContentType) return;
            IsLoading.Value = true;
            using var stream = File.OpenReadStream(MaxFileSize);
            var source = await stream.ConvertToBase64StringAsync();
            _imageSrc = $"data:{File.ContentType};base64,{source}";
        }
        catch (IOException)
        {
            Snackbar.Add("この画像ファイルは設定できません。", Severity.Error);
        }
        finally
        {
            IsLoading.Value = false;
        }
    }
}
