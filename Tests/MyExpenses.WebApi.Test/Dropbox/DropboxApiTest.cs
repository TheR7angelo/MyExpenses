using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using Dropbox.Api;
using Dropbox.Api.Files;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.WebApi.Dropbox;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Dropbox;

public class DropboxApiTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private async Task Test()
    {
        var dropboxService = new DropboxService();
        if (dropboxService.AccessTokenAuthentication is null) dropboxService.AuthorizeApplication();

        if (dropboxService.AccessTokenAuthentication is null) return;

        if (!dropboxService.AccessTokenAuthentication.IsTokenValid())
        {
            testOutputHelper.WriteLine("need to refresh");
        }
        else
        {
            using var client = new DropboxClient(dropboxService.AccessTokenAuthentication.AccessToken);
            var content = $"Hello, World! {DateTime.Now}";
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await client.Files.UploadAsync("/test.txt", WriteMode.Overwrite.Instance, body: memStream);
        }
    }
}