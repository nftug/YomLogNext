using Javax.Net.Ssl;
using Xamarin.Android.Net;

namespace YomLog.MobileApp.Platforms.Android;

public sealed class CustomAndroidMessageHandler : AndroidMessageHandler
{
    protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        => new CustomHostnameVerifier();

    private sealed class CustomHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string? hostname, ISSLSession? session)
            => HttpsURLConnection.DefaultHostnameVerifier?.Verify(hostname, session) == true
               || (session?.PeerPrincipal?.Name == "CN=localhost");
    }
}
