using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.CustomFrame;

public class EPackIconsToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EPackIcons ePackIcons)
        {
            var geometryString = ePackIcons switch
            {
                EPackIcons.Database => "M12,3C7.58,3 4,4.79 4,7C4,9.21 7.58,11 12,11C16.42,11 20,9.21 20,7C20,4.79 16.42,3 12,3M4,9V12C4,14.21 7.58,16 12,16C16.42,16 20,14.21 20,12V9C20,11.21 16.42,13 12,13C7.58,13 4,11.21 4,9M4,14V17C4,19.21 7.58,21 12,21C16.42,21 20,19.21 20,17V14C20,16.21 16.42,18 12,18C7.58,18 4,16.21 4,14Z",
                EPackIcons.DatabaseExport => "M12,3C7.58,3 4,4.79 4,7 4,9.21 7.58,11 12,11 12.5,11 13,10.97 13.5,10.92L13.5,9.5 16.39,9.5 15.39,8.5 18.9,5C17.5,3.8,14.94,3,12,3 M18.92,7.08L17.5,8.5 20,11 15,11 15,13 20,13 17.5,15.5 18.92,16.92 23.84,12 M4,9L4,12C4,14.21 7.58,16 12,16 13.17,16 14.26,15.85 15.25,15.63L16.38,14.5 13.5,14.5 13.5,12.92C13,12.97 12.5,13 12,13 7.58,13 4,11.21 4,9 M4,14L4,17C4,19.21 7.58,21 12,21 14.94,21 17.5,20.2 18.9,19L17,17.1C15.61,17.66 13.9,18 12,18 7.58,18 4,16.21 4,14z",
                EPackIcons.DatabaseImport => "M12,3C8.59,3,5.69,4.07,4.54,5.57L9.79,10.82C10.5,10.93 11.22,11 12,11 16.42,11 20,9.21 20,7 20,4.79 16.42,3 12,3 M3.92,7.08L2.5,8.5 5,11 0,11 0,13 5,13 2.5,15.5 3.92,16.92 8.84,12 M20,9C20,11.21 16.42,13 12,13 11.34,13 10.7,12.95 10.09,12.87L7.62,15.34C8.88,15.75 10.38,16 12,16 16.42,16 20,14.21 20,12 M20,14C20,16.21 16.42,18 12,18 9.72,18 7.67,17.5 6.21,16.75L4.53,18.43C5.68,19.93 8.59,21 12,21 16.42,21 20,19.21 20,17z",
                EPackIcons.Minus => "M19,13L5,13 5,11 19,11 19,13z",
                EPackIcons.Plus => "M19,13L13,13 13,19 11,19 11,13 5,13 5,11 11,11 11,5 13,5 13,11 19,11 19,13z",
                _ => throw new ArgumentOutOfRangeException()
            };

            return PathGeometryConverter.ConvertFromString(geometryString) as Geometry;
        }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    private static class PathGeometryConverter
    {
        // Placeholder for a real implementation. In practice, you would need to parse the string correctly.
        // Here we assume a simple PathGeometry string. You can expand this to handle other types of Geometry.
        public static object? ConvertFromString(string geometryString)
        {
            // Add parsing logic here
            var converter = new Microsoft.Maui.Controls.Shapes.PathGeometryConverter();
            return converter.ConvertFromInvariantString(geometryString) as Geometry;
        }
    }
}