using System.Reflection;
using Dropbox.Api;
using Dropbox.Api.FileProperties;
using Dropbox.Api.Files;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Share.Core.WebApi;
using MyExpenses.Utils;

namespace MyExpenses.WebApi.Dropbox;

public class DropboxService
{
    private AccessTokenAuthentication? AccessTokenAuthentication { get; set; }

    private DropboxKeys DropboxKeys { get; set; }

    private ProjectSystem ProjectSystem { get; init; }

    private DropboxService()
    {
        DropboxKeys = GetDropboxKeys();
    }

    /// <summary>
    /// Deletes multiple files from a specified folder in Dropbox.
    /// </summary>
    /// <param name="filePaths">An array of file paths to be deleted.</param>
    /// <param name="folder">The folder in which the files reside. Default is null, which assumes the root folder or current context.</param>
    /// <returns>An array of <see cref="DeleteResult"/> objects indicating the result of the delete operation for each file. Returns null if any file was not successfully handled.</returns>
    public async Task<DeleteResult?[]> DeleteFilesAsync(string[] filePaths, string? folder = null)
    {
        var deleteTasks = filePaths.Select(filePath => DeleteFileAsync(filePath, folder));
        var results = await Task.WhenAll(deleteTasks);

        return results;
    }

    private async Task<DeleteResult?> DeleteFileAsync(string filePath, string? folder = null)
    {
        folder ??= string.Empty;

        if (!folder.StartsWith('/')) folder = $"/{folder}";

        var metadatas = await ListFileAsync(folder);
        var files = metadatas.Select(s => s.PathDisplay);

        filePath = $"{folder}/{filePath}";

        if (!files.Contains(filePath)) return null;

        using var dropboxClient = await GetDropboxClient();
        var deleteResult = await dropboxClient.Files.DeleteV2Async(filePath);

        return deleteResult;
    }

    public async Task<IEnumerable<Metadata>> ListFileAsync(string? folder = null, bool recursive = false, bool includeMediaInfo = false,
        bool includeDeleted = false, bool includeHasExplicitSharedMembers = false,
        bool includeMountedFolders = true, uint? limit = null, SharedLink? sharedLink = null,
        TemplateFilterBase? includePropertyGroups = null, bool includeNonDownloadableFiles = true)
    {
        folder ??= string.Empty;

        if (!folder.StartsWith('/')) folder = $"/{folder}";

        using var dropboxClient = await GetDropboxClient();
        var list = await dropboxClient.Files.ListFolderAsync(folder, recursive, includeMediaInfo, includeDeleted,
            includeHasExplicitSharedMembers, includeMountedFolders, limit, sharedLink, includePropertyGroups,
            includeNonDownloadableFiles).ConfigureAwait(false);

        return list.Entries.Where(s => s.IsFile);
    }

    public async Task<string> DownloadFileAsync(string filePath, string? destinationFilePath = null, HttpClient? httpClient = null)
    {
        if (ProjectSystem is ProjectSystem.Maui && httpClient is null)
        {
            throw new InvalidOperationException("HttpClient is required for MAUI projects.");
        }

        if (!filePath.StartsWith('/')) filePath = $"/{filePath}";

        using var dropboxClient = await GetDropboxClient(httpClient);
        
        var response = await dropboxClient.Files.DownloadAsync(filePath);
        var stream = await response.GetContentAsStreamAsync();

        if (!string.IsNullOrWhiteSpace(destinationFilePath))
        {
            var directoryPath = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(directoryPath!);
        }
        else
        {
            destinationFilePath = Path.GetFileName(filePath);
        }

        await using var fileStream = File.Create(destinationFilePath);
        await stream.CopyToAsync(fileStream);

        return destinationFilePath;
    }

    public async Task<FileMetadata> UploadFileAsync(string filePath, string? folder = null)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        using var dropboxClient = await GetDropboxClient();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The FileStream is mandatory here because it allows controlled and efficient access to the file,
        // while respecting the need for asynchronous operations. The FileShare.ReadWrite mode ensures that
        // the file can still be shared with other processes during the operation, avoiding access conflicts.
        // This setup is particularly important when working with files that may be read or written to by
        // other applications or parts of your code at the same time.
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The MemoryStream is necessary here to store the file content in memory,
        // while ensuring that the file is not read from disk until it is needed.
        var byteArray = new byte[fileStream.Length];

        var content = new Memory<byte>(byteArray);
        _ = await fileStream.ReadAsync(content, CancellationToken.None).ConfigureAwait(false);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The MemoryStream is necessary here to store the file content in memory,
        // while ensuring that the file is not read from disk until it is needed.
        using var memoryStream = new MemoryStream(byteArray);

        var dropboxFilePath = string.IsNullOrWhiteSpace(folder)
            ? $"/{Path.GetFileName(filePath)}"
            : $"/{folder.Trim('/')}/{Path.GetFileName(filePath)}";

        return await dropboxClient.Files.UploadAsync(dropboxFilePath, WriteMode.Overwrite.Instance,
            body: memoryStream);
    }

    private async Task<DropboxClient> GetDropboxClient(HttpClient? httpClient = null)
    {
        DropboxClient? dropboxClient = null;
        try
        {
            if (!AccessTokenAuthentication!.IsTokenValid())
            {
                await RefreshAccessTokenAuthentication();
            }

            // The DropboxClient is necessary here to interact with the Dropbox API.
            dropboxClient = httpClient is null
                ? new DropboxClient(AccessTokenAuthentication.AccessToken)
                : new DropboxClient(AccessTokenAuthentication.AccessToken, new DropboxClientConfig { HttpClient = httpClient });
            // ReSharper restore HeapView.ObjectAllocation.Evident

            return dropboxClient;
        }
        catch
        {
            dropboxClient?.Dispose();
            throw;
        }
    }

    private async Task RefreshAccessTokenAuthentication()
    {
        using var httpClient = Http.GetHttpClient();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The FormUrlEncodedContent is necessary here to create a request body for the POST request.
        // The request body contains the necessary data to authenticate the application and refresh the access token.
        // The request is sent to the Dropbox API to obtain a new access token.
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AccessTokenAuthentication!.RefreshToken! },
            { "client_id", DropboxKeys.AppKey! },
            { "client_secret", DropboxKeys.AppSecret! }
        };

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The requestContent is necessary here to send the request body to the Dropbox API.
        // The request is sent to the Dropbox API to obtain a new access token.
        var requestContent = new FormUrlEncodedContent(requestData);

        var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        var accessTokenResponse = responseContent.ToObject<AccessTokenAuthentication>();
        AccessTokenAuthentication.AccessToken = accessTokenResponse!.AccessToken;
        AccessTokenAuthentication.ExpiresIn = accessTokenResponse.ExpiresIn;
        AccessTokenAuthentication.TokenType = accessTokenResponse.TokenType;

        var now = DateTime.Now;
        AccessTokenAuthentication.DateCreated = now;
        AccessTokenAuthentication.DateExpiration = now.AddSeconds(accessTokenResponse.ExpiresIn ?? 0);

        await File.WriteAllTextAsync(DropboxServiceUtils.FilePathSecretKeys, AccessTokenAuthentication.ToJson());
    }

    public static async Task<DropboxService> CreateAsync(ProjectSystem projectSystem)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The DropboxService is necessary here to interact with the Dropbox API.
        var service = new DropboxService { ProjectSystem = projectSystem };

        await service.InitializeAsync(projectSystem);
        return service;
    }

    private async Task InitializeAsync(ProjectSystem projectSystem)
    {
        DropboxKeys = GetDropboxKeys();

        if (!File.Exists(DropboxServiceUtils.FilePathSecretKeys))
        {
            AccessTokenAuthentication = await AuthorizeApplicationAsync(projectSystem);
        }
        else
        {
            var jsonStr = await File.ReadAllTextAsync(DropboxServiceUtils.FilePathSecretKeys);
            AccessTokenAuthentication = jsonStr.ToObject<AccessTokenAuthentication>();
            if (AccessTokenAuthentication!.AccessToken is null) AccessTokenAuthentication = await AuthorizeApplicationAsync(projectSystem);
        }
    }

    private async Task<AccessTokenAuthentication?> AuthorizeApplicationAsync(ProjectSystem projectSystem)
    {
        var pkceData = Share.Core.WebApi.Utils.GeneratePkceData();
        
        var authenticator = projectSystem.CreateAuthenticator();
        var tempToken = await authenticator.AuthenticateAsync(DropboxKeys, pkceData);
        if (string.IsNullOrEmpty(tempToken)) return null;

        var accessTokenAuthentication = await GetAccessTokenAuthentication(tempToken, pkceData, projectSystem);

        if (accessTokenAuthentication is not null)
        {
            accessTokenAuthentication.DateCreated = DateTime.Now;
            accessTokenAuthentication.DateExpiration =
                DateTime.Now.AddSeconds(accessTokenAuthentication.ExpiresIn ?? 0);
        }

        await File.WriteAllTextAsync(DropboxServiceUtils.FilePathSecretKeys, accessTokenAuthentication?.ToJson());
        return accessTokenAuthentication;
    }

    private static DropboxKeys GetDropboxKeys()
    {
        var assembly = Assembly.GetAssembly(typeof(DropboxService))!;
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.First(s => s.Contains("DropboxKeys.json"));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The StreamReader is necessary here to read the JSON file and convert it to a dictionary.
        // The JSON file contains the necessary information to authenticate the application with Dropbox.
        using var reader = new StreamReader(stream);

        var jsonStr = reader.ReadToEnd();

        return jsonStr.ToObject<DropboxKeys>()!;
    }

    private async Task<AccessTokenAuthentication?> GetAccessTokenAuthentication(string tempToken, Pkce pkceData,
        ProjectSystem projectSystem)
    {
        var redirectUri = projectSystem is ProjectSystem.Wpf
            ? DropboxKeys.RedirectUriWpf!
            : DropboxKeys.RedirectUri!;

        using var httpClient = Http.GetHttpClient();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The FormUrlEncodedContent is necessary here to create a request body for the POST request.
        // The request body contains the necessary data to authenticate the application and obtain an access token.
        var requestData = new Dictionary<string, string>
        {
            { "code", tempToken },
            { "grant_type", "authorization_code" },
            { "client_id", DropboxKeys.AppKey! },
            { "client_secret", DropboxKeys.AppSecret! },
            { "redirect_uri", redirectUri },
            { "code_verifier", pkceData.CodeVerifier }
        };

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The requestContent is necessary here to send the request body to the Dropbox API.
        // The request is sent to the Dropbox API to obtain an access token.
        var requestContent = new FormUrlEncodedContent(requestData);

        var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", requestContent)
            .ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync();

        var accessTokenResponse = responseContent.ToObject<AccessTokenAuthentication>();
        return accessTokenResponse;
    }
}
