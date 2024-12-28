using MyExpenses.Models.Ui.Maui;

namespace MyExpenses.Smartphones.ColorManipulation;

public static class LabExtensions
{
    public static Lab ToLab(this Color c)
        => c.ToXyz().ToLab();

    private static Lab ToLab(this Xyz xyz)
    {
        var fx = XyzLab(xyz.X / LabConstants.WhitePointX);
        var fy = XyzLab(xyz.Y / LabConstants.WhitePointY);
        var fz = XyzLab(xyz.Z / LabConstants.WhitePointZ);

        var l = 116 * fy - 16;
        var a = 500 * (fx - fy);
        var b = 200 * (fy - fz);
        return new Lab(l, a, b);

        static double XyzLab(double v)
        {
            // v /= 255; disable for Maui
            if (v > LabConstants.e) return Math.Pow(v, 1 / 3.0);
            return (v * LabConstants.k + 16) / 116;
        }
    }

    public static Color ToColor(this Lab lab)
        => lab.ToXyz().ToColor();
}