#if __ANDROID__
using Xamarin.Android.Net;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
#endif

namespace MyExpenses.Maui.Utils.WebApi;

public static class HttpClientHandlerCustom
{
    public static HttpClient CreateHttpClientHandler()
    {
#if __ANDROID__
        // ReSharper disable HeapView.ObjectAllocation.Evident
        return new HttpClient(new HttpClientHandlerAndroid());
        // ReSharper restore HeapView.ObjectAllocation.Evident
#else
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        return new HttpClient();
#endif
    }
}

#if __ANDROID__

public class HttpClientHandlerAndroid : AndroidMessageHandler
{
    [SupportedOSPlatform("Android21.0")]
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri!.AbsolutePath.Contains("files/download"))
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }
        return base.SendAsync(request, cancellationToken);
    }
}

#endif