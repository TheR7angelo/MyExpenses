using System.ComponentModel;
using System.Drawing;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow
{
    public TColor TColor { get; } = new();

    public Color RedSliderStart
    {
        get
        {
            var color = TColor.HexadecimalColorCode?.ToColor();
            int alpha, green, blue;
            if (color is null)
            {
                alpha = 255;
                green = 0;
                blue = 0;
            }
            else
            {
                alpha = color.Value.A;
                green = color.Value.G;
                blue = color.Value.B;
            }

            var startColor = Color.FromArgb(alpha, 0, green, blue);
            return startColor;
        }
    }

    public Color RedSliderStop
    {
        get
        {
            var color = TColor.HexadecimalColorCode?.ToColor();
            var endColor = Color.FromArgb(color?.A ?? 255, 255, color?.G ?? 0, color?.B ?? 0);
            return endColor;
        }
    }

    public AddEditColorWindow()
    {
        InitializeComponent();

        TColor.PropertyChanged += ColorOnPropertyChanged;
    }

    private void ColorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine("hey");
    }
}