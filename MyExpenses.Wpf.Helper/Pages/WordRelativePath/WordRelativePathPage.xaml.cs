using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;

namespace MyExpenses.Wpf.Helper.Pages.WordRelativePath;

public partial class WordRelativePathPage
{
    public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(nameof(FilePath),
        typeof(string), typeof(WordRelativePathPage), new PropertyMetadata(default(string)));

    public string FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public WordRelativePathPage()
    {
        InitializeComponent();
    }

    private void ButtonSelectWord_OnClick(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog { Multiselect = false, Filter = "Word file (.docx)|*.docx" };

        var result = fileDialog.ShowDialog();
        if (result is not true) return;

        FilePath = fileDialog.FileName;
    }

    private void ButtonTransform_OnClick(object sender, RoutedEventArgs e)
    {
        var temp = Path.GetFullPath("temp.zip");

        var now = DateTime.Now;
        var directory = Path.GetDirectoryName(FilePath)!;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
        var filePath = Path.Join(directory, $"{fileNameWithoutExtension}_{now:yyyy-MM-dd}.docx");

        File.Copy(FilePath, temp, true);

        EditZipArchive(temp);

        File.Move(temp, filePath, true);
    }

    private static void EditZipArchive(string temp)
    {
        using var fileStream = new FileStream(temp, FileMode.Open);
        using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Update);

        var entries = zipArchive.Entries.Where(x => x.FullName.StartsWith("word/media")).ToList();
        foreach (var entry in entries)
        {
            entry.Delete();
        }

        // var xml = zipArchive.Entries.FirstOrDefault(x => x.FullName.Equals("word/_rels/document.xml.rels"));
        // if (xml is null) return;
        //
        // var entryStream = xml.Open();
        // var reader = new StreamReader(entryStream);
        // var document = XDocument.Load(reader);
        //
        // var relationShips = document.Element(XName.Get("Relationships",
        //     "http://schemas.openxmlformats.org/package/2006/relationships"));
        //
        // var wordDirectory = new DirectoryInfo(Path.GetDirectoryName(FilePath)!).Name.Replace(" ", "%20");
        // foreach (var relationShip in relationShips!.Elements(XName.Get("Relationship", "http://schemas.openxmlformats.org/package/2006/relationships")))
        // {
        // var targetAttr = relationShip.Attribute("Target");
        // if (targetAttr is null) continue;
        //
        // var value = targetAttr.Value;
        // if (value.StartsWith("file:"))
        // {
        //     var relativePath = value.Split(wordDirectory).Last();
        //     relativePath = $"{wordDirectory}{relativePath}";
        //     targetAttr.Value = relativePath;
        // }
        // }
        //
        // var writer = new StreamWriter(entryStream);
        // document.Save(writer);
        // writer.Flush();
    }
}