#if __ANDROID__
using Xamarin.Android.Net;
using System.Net.Http.Headers;
#endif

namespace MyExpenses.Maui.Utils.WebApi;

public static class HttpClientHandlerCustom
{
    public static HttpClient CreateHttpClientHandler()
    {
#if __ANDROID__
        return new HttpClient(new HttpClientHandlerAndroid());
#else
        return new HttpClient();
#endif
    }
}

#if __ANDROID__

public class HttpClientHandlerAndroid : AndroidMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri!.AbsolutePath.Contains("files/download"))
        {
            request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }
        return base.SendAsync(request, cancellationToken);
    }
}

#endif