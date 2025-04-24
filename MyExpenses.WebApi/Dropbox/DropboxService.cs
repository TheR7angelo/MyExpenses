using System.Reflection;
using Dropbox.Api;
using Dropbox.Api.FileProperties;
using Dropbox.Api.Files;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Share.Core.WebApi;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.WebApi.Dropbox;

/// <summary>
/// The DropboxService class provides methods for interacting with the Dropbox API,
/// allowing operations such as uploading, downloading, listing, and deleting files.
/// </summary>
public class DropboxService
{
    /// <summary>
    /// Represents the property used to manage access token authentication within the DropboxService class.
    /// This property stores the authentication information required to interact with the Dropbox API,
    /// including the access token, token type, creation date, and expiration date of the token.
    /// The property ensures that the token is valid and can be refreshed when needed.
    /// </summary>
    private AccessTokenAuthentication? AccessTokenAuthentication { get; set; }

    /// <summary>
    /// Represents the property used to store the API keys and secrets required for authenticating and interacting with the Dropbox API.
    /// This property encapsulates critical configuration details, such as application credentials,
    /// which are used across the DropboxService class for various API operations including authentication,
    /// token management, and request execution.
    /// </summary>
    private DropboxKeys DropboxKeys { get; set; }

    /// <summary>
    /// Represents the project system configuration used by the DropboxService class.
    /// This property indicates the targeted application platform, such as WPF or MAUI,
    /// for which the DropboxService is being utilized. It may alter behavior based on
    /// the specific requirements or limitations of the selected project system.
    /// </summary>
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
        // ReSharper disable once HeapView.ObjectAllocation
        var deleteTasks = GetAllDeleteTasks(filePaths, folder);
        var results = await Task.WhenAll(deleteTasks);

        return results;
    }

    /// <summary>
    /// Creates a collection of asynchronous tasks to delete files from a specified folder in Dropbox.
    /// </summary>
    /// <param name="filePaths">A collection of file paths to be deleted.</param>
    /// <param name="folder">The folder in which the files reside. Default is null, which assumes the root folder or current context.</param>
    /// <returns>A collection of tasks that represent the asynchronous delete operations for each file. Each task returns a <see cref="DeleteResult"/> object indicating the result of the delete operation, or null if the operation was unsuccessful.</returns>
    private IEnumerable<Task<DeleteResult?>> GetAllDeleteTasks(IEnumerable<string> filePaths, string? folder)
    {
        foreach (var filePath in filePaths)
        {
            yield return DeleteFileAsync(filePath, folder);
        }
    }

    /// <summary>
    /// Deletes a single file from a specified folder in Dropbox.
    /// </summary>
    /// <param name="filePath">The path of the file to be deleted.</param>
    /// <param name="folder">The folder in which the file resides. Default is null, which assumes the root folder or current context.</param>
    /// <returns>A <see cref="DeleteResult"/> object indicating the result of the delete operation for the file. Returns null if the file was not found or the operation failed.</returns>
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

    /// <summary>
    /// Lists all files within a specified folder in Dropbox based on the given options.
    /// </summary>
    /// <param name="folder">The folder path to list files from. Default is null, which targets the root folder.</param>
    /// <param name="recursive">Specifies whether to recursively list files in all subfolders. Default is false.</param>
    /// <param name="includeMediaInfo">Indicates whether to include media information in the results. Default is false.</param>
    /// <param name="includeDeleted">Specifies whether to include deleted files in the results. Default is false.</param>
    /// <param name="includeHasExplicitSharedMembers">Indicates whether to include a flag for files with explicit shared members. Default is false.</param>
    /// <param name="includeMountedFolders">Determines whether to include mounted folders in the results. Default is true.</param>
    /// <param name="limit">Limits the number of entries returned, if specified. Default is null, meaning no limit.</param>
    /// <param name="sharedLink">A shared link object to list contents within the link context. Default is null.</param>
    /// <param name="includePropertyGroups">Specifies property groups to include in the results. Default is null.</param>
    /// <param name="includeNonDownloadableFiles">Indicates whether to include files that cannot be downloaded. Default is true.</param>
    /// <returns>A collection of <see cref="Metadata"/> objects representing the listed files.</returns>
    public async Task<IEnumerable<Metadata>> ListFileAsync(string? folder = null, bool recursive = false,
        bool includeMediaInfo = false,
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

    /// <summary>
    /// Downloads a file from Dropbox and optionally saves it to the specified destination path.
    /// </summary>
    /// <param name="filePath">The path of the file to download from Dropbox.</param>
    /// <param name="destinationFilePath">The full file path where the downloaded file will be saved. If null, the file will be saved with its original name in the current directory.</param>
    /// <param name="httpClient">An optional custom instance of <see cref="HttpClient"/>. Required for MAUI projects.</param>
    /// <returns>The full file path where the downloaded file was saved.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the application is running on a MAUI project and no <see cref="HttpClient"/> is provided.</exception>
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

    /// <summary>
    /// Uploads a file to the specified folder in Dropbox.
    /// </summary>
    /// <param name="filePath">The path of the file to be uploaded.</param>
    /// <param name="folder">The Dropbox folder where the file will be stored. Default is null, which uploads the file to the root folder.</param>
    /// <returns>A <see cref="FileMetadata"/> object containing metadata of the uploaded file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
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

    /// <summary>
    /// Retrieves a DropboxClient instance to interact with the Dropbox API using the current authentication token.
    /// </summary>
    /// <param name="httpClient">An optional <see cref="HttpClient"/> instance to use for API requests. If not provided, a new instance is created internally.</param>
    /// <returns>A <see cref="DropboxClient"/> instance initialized with valid authentication credentials.</returns>
    private async Task<DropboxClient> GetDropboxClient(HttpClient? httpClient = null)
    {
        Log.Information("Initializing Dropbox client");
        DropboxClient? dropboxClient = null;
        try
        {
            if (!AccessTokenAuthentication!.IsTokenValid())
            {
                Log.Information("Access token expired or invalid. Refreshing token");
                await RefreshAccessTokenAuthentication();
                Log.Information("Access token successfully refreshed");
            }
            else
            {
                Log.Information("The token is still valid until {DateExpiration}", AccessTokenAuthentication.DateExpiration?.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            Log.Information("Creating Dropbox client {WithCustomHttpClient}", httpClient != null ? "with custom HTTP client" : "with default HTTP client");

            // ReSharper disable HeapView.ObjectAllocation.Evident
            // The DropboxClient is necessary here to interact with the Dropbox API.
            dropboxClient = httpClient is null
                ? new DropboxClient(AccessTokenAuthentication.AccessToken)
                : new DropboxClient(AccessTokenAuthentication.AccessToken, new DropboxClientConfig { HttpClient = httpClient });
            // ReSharper restore HeapView.ObjectAllocation.Evident

            Log.Information("Dropbox client successfully created");
            return dropboxClient;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while creating Dropbox client");
            dropboxClient?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Refreshes the access token used for Dropbox API authentication by generating a new token
    /// using the refresh token and updating the current access token details.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of refreshing the access token. Returns nothing when the process is complete.</returns>
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

    /// <summary>
    /// Creates and initializes an instance of <see cref="DropboxService"/> for interacting with the Dropbox API.
    /// </summary>
    /// <param name="projectSystem">The project system for which the Dropbox service is being initialized, such as Wpf or Maui.</param>
    /// <returns>An initialized instance of <see cref="DropboxService"/> ready for use.</returns>
    public static async Task<DropboxService> CreateAsync(ProjectSystem projectSystem)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The DropboxService is necessary here to interact with the Dropbox API.
        var service = new DropboxService { ProjectSystem = projectSystem };

        await service.InitializeAsync(projectSystem);
        return service;
    }

    /// <summary>
    /// Initializes the DropboxService by acquiring Dropbox keys and setting up the access token authentication.
    /// </summary>
    /// <param name="projectSystem">The specific project system (e.g., Wpf or Maui) that determines the context of the initialization.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    private async Task InitializeAsync(ProjectSystem projectSystem)
    {
        Log.Information("Starting Dropbox service initialization");

        DropboxKeys = GetDropboxKeys();
        Log.Information("Dropbox keys retrieved");

        if (!File.Exists(DropboxServiceUtils.FilePathSecretKeys))
        {
            Log.Information("Secret keys file not found. Starting application authorization");
            AccessTokenAuthentication = await AuthorizeApplicationAsync(projectSystem);
        }
        else
        {
            Log.Information("Reading existing secret keys file");
            var jsonStr = await File.ReadAllTextAsync(DropboxServiceUtils.FilePathSecretKeys);
            AccessTokenAuthentication = jsonStr.ToObject<AccessTokenAuthentication>();
            if (AccessTokenAuthentication!.AccessToken is null)
            {
                Log.Warning("Access token not found in file. New authorization required");
                AccessTokenAuthentication = await AuthorizeApplicationAsync(projectSystem);
            }
            else
            {
                Log.Information("Access token successfully retrieved from file");
            }

            Log.Information("Dropbox service initialization completed");
        }
    }

    /// <summary>
    /// Authenticates the application with Dropbox and retrieves an access token.
    /// </summary>
    /// <param name="projectSystem">The platform-specific project system used for generating the authentication process (e.g., WPF or MAUI).</param>
    /// <returns>An <see cref="AccessTokenAuthentication"/> object containing the access token and metadata, or null if the authentication fails.</returns>
    private async Task<AccessTokenAuthentication?> AuthorizeApplicationAsync(ProjectSystem projectSystem)
    {
        Log.Information("Starting application authorization process");

        var pkceData = Share.Core.WebApi.Utils.GeneratePkceData();
        Log.Information("PKCE data generated");
        
        var authenticator = projectSystem.CreateAuthenticator();
        Log.Information("Created authenticator for project system: {ProjectSystem}", projectSystem);

        var tempToken = await authenticator.AuthenticateAsync(DropboxKeys, pkceData);
        if (string.IsNullOrEmpty(tempToken))
        {
            Log.Warning("Failed to obtain temporary token. Authentication aborted");
            return null;
        }

        Log.Information("Temporary token obtained successfully");
        var accessTokenAuthentication = await GetAccessTokenAuthentication(tempToken, pkceData, projectSystem);

        if (accessTokenAuthentication is not null)
        {
            Log.Information("Access token authentication received");
            accessTokenAuthentication.DateCreated = DateTime.Now;
            accessTokenAuthentication.DateExpiration =
                DateTime.Now.AddSeconds(accessTokenAuthentication.ExpiresIn ?? 0);
            Log.Information("Token expiration set to: {ExpirationDate}", accessTokenAuthentication.DateExpiration);
        }
        else
        {
            Log.Warning("Failed to obtain access token authentication");
        }

        await File.WriteAllTextAsync(DropboxServiceUtils.FilePathSecretKeys, accessTokenAuthentication?.ToJson());
        Log.Information("Authentication data saved to secret keys file");

        return accessTokenAuthentication;
    }

    /// <summary>
    /// Loads and retrieves the DropboxKeys necessary for authenticating with the Dropbox API.
    /// The keys are fetched from an embedded JSON resource within the assembly.
    /// </summary>
    /// <returns>An instance of <see cref="DropboxKeys"/> containing app key, app secret, and redirect URIs required for authentication.</returns>
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

    /// <summary>
    /// Exchanges a temporary token with Dropbox for an access token using the OAuth 2.0 authorization code flow.
    /// </summary>
    /// <param name="tempToken">The temporary authorization code received during the initial authorization step.</param>
    /// <param name="pkceData">The PKCE (Proof Key for Code Exchange) data used for enhanced security during the authorization process.</param>
    /// <param name="projectSystem">The project system specifying the platform (e.g., WPF or MAUI) to determine the appropriate redirect URI.</param>
    /// <returns>An <see cref="AccessTokenAuthentication"/> object containing the access token and related authentication data, or null if the process fails.</returns>
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
