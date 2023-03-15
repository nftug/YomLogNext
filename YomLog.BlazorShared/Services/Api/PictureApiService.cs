using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using YomLog.BlazorShared.Models;
using YomLog.Shared.Extensions;

namespace YomLog.BlazorShared.Services.Api;

[InjectAsScoped]
public class PictureApiService
{
    private readonly HttpClientWrapper _httpClientWrapper;
    private readonly AppSettings _appSettings;

    public PictureApiService(HttpClientWrapper httpClientWrapper, AppSettings appSettings)
    {
        _httpClientWrapper = httpClientWrapper;
        _appSettings = appSettings;
    }

    public string GetPicturePath(Guid pictureId)
        => $"{_appSettings.ApiBaseAddress.AbsoluteUri}pictures/{pictureId}.jpg";

    public async Task UploadAsync(IBrowserFile file, Guid pictureId)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: long.MaxValue));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(content: fileContent, name: "\"file\"", fileName: $"{pictureId}.jpg");

        await _httpClientWrapper.CreateWithoutResult("pictures", content);
    }
}
