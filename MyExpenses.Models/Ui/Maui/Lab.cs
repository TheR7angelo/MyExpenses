namespace MyExpenses.Models.Ui.Maui;

public record struct Lab(double L, double A, double B);

public static class LabConstants
{
    public const double Kn = 18;

    public const double WhitePointX = 0.95047;
    public const double WhitePointY = 1;
    public const double WhitePointZ = 1.08883;

    public static readonly double ECubedRoot = Math.Pow(e, 1.0 / 3);
    public const double k = 24389 / 27.0;
    public const double e = 216 / 24389.0;
}