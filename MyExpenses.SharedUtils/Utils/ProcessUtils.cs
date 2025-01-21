using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyExpenses.SharedUtils.Utils;

/// <summary>
/// Utility class for starting processes or opening system-level files and folders.
/// </summary>
public static class ProcessUtils
{
    /// <summary>
    /// Opens the specified process.
    /// </summary>
    /// <param name="process">The path or name of the process to be opened.</param>
    /// <remarks>
    /// This method opens the specified process using the default program associated with it.
    /// </remarks>
    public static void StartProcess(this string process)
    {
        Process.Start(new ProcessStartInfo(process) { UseShellExecute = true });
    }

    /// <summary>
    /// Starts a process with the specified file and parameters.
    /// </summary>
    /// <param name="filename">The path or name of the file to start as a process.</param>
    /// <param name="useShellExecute">Indicates whether to use the operating system shell to start the process.</param>
    /// <param name="createNoWindow">Indicates whether to start the process without creating a new window.</param>
    /// <param name="windowStyle">Specifies the style of the window for the process.</param>
    /// <remarks>
    /// This method starts a process with customizable execution options.
    /// </remarks>
    public static void StartProcessWithParameters(this string filename, bool useShellExecute = true,
        bool createNoWindow = false,
        ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
    {
        var process = new Process();
        process.StartInfo.UseShellExecute = useShellExecute;
        process.StartInfo.CreateNoWindow = createNoWindow;
        process.StartInfo.WindowStyle = windowStyle;
        process.StartInfo.FileName = filename;
        process.Start();
    }

    /// <summary>
    /// Opens the specified file or folder in the default system file explorer or viewer.
    /// </summary>
    /// <param name="path">The path of the file or folder to open.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the specified folder does not exist.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when the operation is not supported on the current platform.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file or folder cannot be opened due to an unexpected error.</exception>
    public static void StartFile(this string path)
    {
        var isFile = File.Exists(path);
        var isDirectory = Directory.Exists(path);

        if (!isFile && !isDirectory)
        {
            throw new FileNotFoundException($"The path '{path}' does not exist as a file or directory.");
        }

        string command;
        string arguments;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            command = "explorer";
            arguments = isFile ? $"\"{path}\"" : path;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            command = "xdg-open";
            arguments = path;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            command = "open";
            arguments = path;
        }
        else
        {
            throw new PlatformNotSupportedException(
                $"This platform ({RuntimeInformation.OSDescription}) is not supported.");
        }

        try
        {
            Process.Start(new ProcessStartInfo(command, arguments) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            var type = isFile ? "file" : "directory";
            throw new InvalidOperationException($"Failed to open the {type}.", ex);
        }
    }
}