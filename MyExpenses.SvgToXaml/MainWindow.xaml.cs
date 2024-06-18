﻿using System.IO;
using System.Windows.Markup;
using System.Xml.Linq;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace MyExpenses.SvgToXaml;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        MainConversion();
    }

    private static void MainConversion()
    {
        var settings = new WpfDrawingSettings();
        using var reader = new FileSvgReader(settings);

        const string directoryPath = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Wpf\Resources\Assets\Cards";
        var files = Directory.GetFiles(directoryPath, "*.svg");

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

        var directoryName = Path.GetFileName(directoryPath);
        var outputFileName = $"{directoryName}.xaml";
        var filePaths = Directory.GetFiles(directoryPath, "*.xaml").Where(s => Path.GetFileName(s) != outputFileName); // Get all .xaml files in the directory

        var resourceDict = new XElement(ns + "ResourceDictionary",
            new XAttribute(XNamespace.Xmlns + "x", xNs));
        var mergedDicts = new XElement(ns + "ResourceDictionary.MergedDictionaries");
        resourceDict.Add(mergedDicts);

        foreach (var file in filePaths)
        {
            var fileName = Path.GetFileName(file);
            var resourceDictionary = new XElement(ns + "ResourceDictionary",
                new XAttribute("Source", fileName));
            mergedDicts.Add(resourceDictionary);
        }


        var outputPath = Path.Join(directoryPath, outputFileName);

        File.WriteAllText(outputPath, resourceDict.ToString());
    }
}