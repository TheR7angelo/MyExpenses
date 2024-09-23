using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace MyExpenses.Wpf.CleanRelease;

public partial class MainWindow
{
    public static readonly DependencyProperty PathDirectoryProperty = DependencyProperty.Register(nameof(PathDirectory),
        typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

    public string PathDirectory
    {
        get => (string)GetValue(PathDirectoryProperty);
        set => SetValue(PathDirectoryProperty, value);
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ButtonClean_OnClick(object sender, RoutedEventArgs e)
        => Clean();

    private void ButtonSelectDirectory_OnClick(object sender, RoutedEventArgs e)
    {
        var folderBrowser = new OpenFolderDialog { Multiselect = false };

        var result = folderBrowser.ShowDialog();
        if (result is not true) return;

        PathDirectory = folderBrowser.FolderName;
    }

    private void Clean()
    {
        if (string.IsNullOrEmpty(PathDirectory) || !Directory.Exists(PathDirectory)) return;

        DeletePdbFiles();
        DeleteXmlFiles();
        DeleteJsonFiles();
    }

    private void DeleteJsonFiles()
    {
        var jsonFiles = Directory.GetFiles(PathDirectory, "*.deps.json", SearchOption.TopDirectoryOnly)
            .Concat(Directory.GetFiles(PathDirectory, "*.runtimeconfig.json", SearchOption.TopDirectoryOnly))
            .ToArray();

        DeleteFiles(jsonFiles, "JSON");
    }

    private void DeleteXmlFiles()
    {
        var xmlFiles = Directory.GetFiles(PathDirectory, "*.WebView2.*", SearchOption.TopDirectoryOnly)
            .Where(f => f.EndsWith(".xml"))
            .ToArray();

        DeleteFiles(xmlFiles, "XML");
    }

    private void DeletePdbFiles()
    {
        var pdbFiles = Directory.GetFiles(PathDirectory, "*.pdb", SearchOption.TopDirectoryOnly);
        DeleteFiles(pdbFiles, "PDB");
    }

    private static void DeleteFiles(string[] files, string fileType)
    {
        try
        {
            foreach (var filePath in files)
            {
                File.Delete(filePath);
                Console.WriteLine($"{fileType} file deleted: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting {fileType} files: {ex.Message}");
        }
    }
}