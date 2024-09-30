using MyExpenses.Models.Ui.Maui;

namespace MyExpenses.Smartphones.ColorManipulation;

public static class XyzExtensions
{
    public static Color ToColor(this Xyz xyz)
    {
        var r = XyzRgb(3.2404542 * xyz.X - 1.5371385 * xyz.Y - 0.4985314 * xyz.Z);
        var g = XyzRgb(-0.9692660 * xyz.X + 1.8760108 * xyz.Y + 0.0415560 * xyz.Z);
        var b = XyzRgb(0.0556434 * xyz.X - 0.2040259 * xyz.Y + 1.0572252 * xyz.Z);

        return Color.FromRgb(Clip(r), Clip(g), Clip(b));

        byte Clip(double d)
        {
            return d switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => (byte)Math.Round(d)
            };
        }

        double XyzRgb(double d)
        {
            if (d > 0.0031308) return 255.0 * (1.055 * Math.Pow(d, 1.0 / 2.4) - 0.055);
            return 255.0 * (12.92 * d);
        }
    }
    public static Xyz ToXyz(this Color c)
    {
        var r = RgbXyz(c.Red);
        var g = RgbXyz(c.Green);
        var b = RgbXyz(c.Blue);

        var x = 0.4124564 * r + 0.3575761 * g + 0.1804375 * b;
        var y = 0.2126729 * r + 0.7151522 * g + 0.0721750 * b;
        var z = 0.0193339 * r + 0.1191920 * g + 0.9503041 * b;
        return new Xyz(x, y, z);

        static double RgbXyz(double v)
        {
            if (v > 0.04045) return Math.Pow((v + 0.055) / 1.055, 2.4);
            return v / 12.92;
        }
    }

    public static Xyz ToXyz(this Lab lab)
    {
        var y = (lab.L + 16.0) / 116.0;
        var x = double.IsNaN(lab.A) ? y : y + lab.A / 500.0;
        var z = double.IsNaN(lab.B) ? y : y - lab.B / 200.0;

        y = LabConstants.WhitePointY * LabXyz(y);
        x = LabConstants.WhitePointX * LabXyz(x);
        z = LabConstants.WhitePointZ * LabXyz(z);

        return new Xyz(x, y, z);

        static double LabXyz(double d)
        {
            if (d > LabConstants.ECubedRoot) return d * d * d;
            return (116 * d - 16) / LabConstants.k;
        }
    }
}