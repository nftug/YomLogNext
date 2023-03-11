// Reference: https://github.com/joseasync/MauiMaueFood

using System.Diagnostics;
using IdentityModel.OidcClient.Browser;
using IBrowser = IdentityModel.OidcClient.Browser.IBrowser;

namespace YomLog.MobileApp.Services;

public class WebAuthenticatorBrowser : IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var authResult =
                await WebAuthenticator.AuthenticateAsync(new Uri(options.StartUrl), new Uri(options.EndUrl));
            var authorizeResponse = ToRawIdentityUrl(options.EndUrl, authResult);

            return new BrowserResult { Response = authorizeResponse };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new BrowserResult()
            {
                ResultType = BrowserResultType.UnknownError,
                Error = ex.ToString()
            };
        }
    }

    public static string ToRawIdentityUrl(string redirectUrl, WebAuthenticatorResult result)
    {
        IEnumerable<string> parameters = result.Properties.Select(pair => $"{pair.Key}={pair.Value}");
        var values = string.Join("&", parameters);

        return $"{redirectUrl}#{values}";
    }
}
