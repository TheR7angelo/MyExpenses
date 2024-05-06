using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Converters;

public class MsgBoxImageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not MsgBoxImage icon) return PackIconKind.None;
        return icon switch
        {
            MsgBoxImage.Error => PackIconKind.Error,
            MsgBoxImage.Hand => PackIconKind.Hand,
            MsgBoxImage.Stop => PackIconKind.Stop,
            MsgBoxImage.Question => PackIconKind.QuestionMark,
            MsgBoxImage.Exclamation => PackIconKind.Exclamation,
            MsgBoxImage.Warning => PackIconKind.Warning,
            MsgBoxImage.Asterisk => PackIconKind.Asterisk,
            MsgBoxImage.Information => PackIconKind.Information,
            _ => PackIconKind.None
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}