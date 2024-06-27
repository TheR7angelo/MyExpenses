using System.Text;
using Dropbox.Api;
using Dropbox.Api.Files;
using MyExpenses.WebApi.Dropbox;
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

        Assert.NotNull(dropboxService.AccessTokenAuthentication);

        if (!dropboxService.AccessTokenAuthentication.IsTokenValid())
        {
            testOutputHelper.WriteLine("need to refresh");
        }
        else
        {
            using var dropboxClient = new DropboxClient(dropboxService.AccessTokenAuthentication.AccessToken);
            var content = $"Hello, World! {DateTime.Now}";
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await dropboxClient.Files.UploadAsync("/test.txt", WriteMode.Overwrite.Instance, body: memStream);
        }
    }
}