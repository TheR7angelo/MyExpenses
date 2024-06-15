using System.Windows.Media;
using MyExpenses.Wpf.Utils;
using SkiaSharp;

namespace MyExpenses.Wpf.UserControls.Colors;

public class ColorChangedEventArgs(Color newColor) : EventArgs
{
    public Color Color { get; private set; } = newColor;
    public SKColor SkColor { get; private set; } = newColor.ToSkColor();
    public string HexadecimalCode { get; private set; } = newColor.ToHexadecimal();
}