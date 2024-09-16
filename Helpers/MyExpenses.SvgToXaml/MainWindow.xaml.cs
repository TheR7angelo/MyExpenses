using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace MyExpenses.SvgToXaml;

public partial class MainWindow
{
    public static readonly DependencyProperty PathDirectoryProperty = DependencyProperty.Register(nameof(PathDirectory),
        typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

    public MainWindow()
    {
        InitializeComponent();
    }

    public string PathDirectory
    {
        get => (string)GetValue(PathDirectoryProperty);
        set => SetValue(PathDirectoryProperty, value);
    }

    private void ButtonTransform_OnClick(object sender, RoutedEventArgs e)
        => MainConversion();

    private void ButtonSelectDirectory_OnClick(object sender, RoutedEventArgs e)
    {
        var folderBrowser = new OpenFolderDialog { Multiselect = false };

        var result = folderBrowser.ShowDialog();
        if (result is not true) return;

        PathDirectory = folderBrowser.FolderName;
    }

    private void MainConversion()
    {
        var settings = new WpfDrawingSettings();
        using var reader = new FileSvgReader(settings);

        var files = Directory.GetFiles(PathDirectory, "*.svg");

        var xSvg = XNamespace.Get("http://sharpvectors.codeplex.com/runtime/");
        var ns = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        var xNs = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");

        foreach (var file in files)
        {
            var key = Path.GetFileNameWithoutExtension(file);

            var drawing = reader.Read(file);
            var xamlDrawing = XamlWriter.Save(drawing);

            var drawingElement = XElement.Parse(xamlDrawing);
            drawingElement = drawingElement.Descendants().First(s => s.Name.LocalName.Equals("DrawingGroup"));

            var resourceDictionary = new XElement(ns + "ResourceDictionary",
                new XAttribute(XNamespace.Xmlns + "x", xNs));
            var drawingImageElement = new XElement(ns + "DrawingImage", new XAttribute(xNs + "Key", key));
            var drawingContainerElement = new XElement(ns + "DrawingImage.Drawing");

            resourceDictionary.Add(drawingImageElement);
            drawingImageElement.Add(drawingContainerElement);
            drawingContainerElement.Add(drawingElement);

            foreach (var xElement in resourceDictionary.Descendants())
            {
                xElement.Attributes(xSvg + "SvgLink.Key").Remove();
                xElement.Attributes(xSvg + "SvgObject.UniqueId").Remove();
                xElement.Attributes(xSvg + "SvgObject.Class").Remove();

                var penAttribute = xElement.Attribute("Pen");
                if (penAttribute is not null && penAttribute.Value.Equals("{x:Null}"))
                {
                    penAttribute.Remove();
                }
            }

            var xamlStr = resourceDictionary.ToString(SaveOptions.None);
            xamlStr = xamlStr.Replace("xmlns:svg=\"http://sharpvectors.codeplex.com/runtime/\"", "");

            var xamlFile = Path.ChangeExtension(file, ".xaml");
            File.WriteAllText(xamlFile, xamlStr);
        }

        var directoryName = Path.GetFileName(PathDirectory);
        var outputFileName = $"{directoryName}.xaml";
        var filePaths = Directory.GetFiles(PathDirectory, "*.xaml")
            .Where(s => Path.GetFileName(s) != outputFileName); // Get all .xaml files in the directory

        var resourceDict = new XElement(ns + "ResourceDictionary",
            new XAttribute(XNamespace.Xmlns + "x", xNs));
        var mergedDictionaries = new XElement(ns + "ResourceDictionary.MergedDictionaries");
        resourceDict.Add(mergedDictionaries);

        foreach (var file in filePaths)
        {
            var fileName = Path.GetFileName(file);
            var resourceDictionary = new XElement(ns + "ResourceDictionary",
                new XAttribute("Source", fileName));
            mergedDictionaries.Add(resourceDictionary);
        }


        var outputPath = Path.Join(PathDirectory, outputFileName);

        File.WriteAllText(outputPath, resourceDict.ToString());
    }
}