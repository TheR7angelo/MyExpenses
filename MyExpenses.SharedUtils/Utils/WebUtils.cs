namespace MyExpenses.SharedUtils.Utils;

public static class WebUtils
{
    /// <summary>
    /// Opens the MyExpenses GitHub page.
    /// </summary>
    /// <remarks>
    /// This method opens the web page for the MyExpenses project on GitHub.
    /// </remarks>
    /// <seealso cref="ProcessUtils.StartProcess(string)"/>
    public static void OpenGithubPage()
    {
        const string url = "https://github.com/TheR7angelo/MyExpenses";
        url.StartProcess();
    }
}