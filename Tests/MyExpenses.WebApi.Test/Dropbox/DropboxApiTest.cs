using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Dropbox;

public class DropboxApiTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private void Test()
    {
        // var dropboxService = new DropboxService();
        // if (dropboxService.AccessTokenAuthentication is null) dropboxService.AuthorizeApplication(ProjectSystem.Wpf);
        //
        // if (dropboxService.AccessTokenAuthentication is null) return;
        //
        // Assert.NotNull(dropboxService.AccessTokenAuthentication);
        //
        // if (!dropboxService.AccessTokenAuthentication.IsTokenValid())
        // {
        //     testOutputHelper.WriteLine("need to refresh");
        //
        //     await dropboxService.RefreshAccessTokenAuthentication();
        // }
        //
        // var filePath = Path.GetFullPath("test.txt");
        // await File.WriteAllTextAsync(filePath, $"Hello, World! {DateTime.Now}");
        // var fileMetadata = await dropboxService.UploadFileAsync(filePath, null);
        //
        // Assert.NotNull(fileMetadata);
        //
        // filePath = await dropboxService.DownloadFileAsync(fileMetadata.PathDisplay);
        // Assert.True(File.Exists(filePath));
    }
}