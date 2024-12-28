using MyExpenses.Models.Ui.Maui;

namespace MyExpenses.Smartphones.ColorManipulation;

public static class ColorAssist
{
    private static float RelativeLuminance(this Color color)
    {
        // Disable for Maui
        // return
        //     0.2126f * Calc(color.Red / 255f) +
        //     0.7152f * Calc(color.Green / 255f) +
        //     0.0722f * Calc(color.Blue / 255f);

        return
            0.2126f * Calc(color.Red) +
            0.7152f * Calc(color.Green) +
            0.0722f * Calc(color.Blue);

        static float Calc(float colorValue)
            => colorValue <= 0.03928f ? colorValue / 12.92f : (float)Math.Pow((colorValue + 0.055f) / 1.055f, 2.4);
    }

    /// <summary>
    /// The contrast ratio is calculated as (L1 + 0.05) / (L2 + 0.05), where
    /// L1 is the: relative luminance of the lighter of the colors, and
    /// L2 is the relative luminance of the darker of the colors.
    /// Based on https://www.w3.org/TR/2008/REC-WCAG20-20081211/#contrast%20ratio
    /// </summary>
    /// <param name="color"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    private static float ContrastRatio(this Color color, Color color2)
    {
        var l1 = color.RelativeLuminance();
        var l2 = color2.RelativeLuminance();
        if (l2 > l1)
        {
            (l1, l2) = (l2, l1);
        }
        return (l1 + 0.05f) / (l2 + 0.05f);
    }

    /// <summary>
    /// Adjust the foreground color to have an acceptable contrast ratio.
    /// </summary>
    /// <param name="foreground">The foreground color</param>
    /// <param name="background">The background color</param>
    /// <param name="targetRatio">The target contrast ratio</param>
    /// <param name="tolerance">The tolerance to the contrast ratio needs to be within</param>
    /// <returns>The updated foreground color with the target contrast ratio with the background</returns>
    public static Color EnsureContrastRatio(this Color foreground, Color background, float targetRatio, float tolerance = 0.1f)
    {
        double offset = 0.0f;

        var ratio = foreground.ContrastRatio(background);
        if (ratio > targetRatio) return foreground;

        var shouldDarken = ShouldDarken(background);

        return AdjustColorToTargetRatio(foreground, background, targetRatio, ref offset, shouldDarken, tolerance);
    }

    private static bool ShouldDarken(Color background)
    {
        var contrastWithWhite = background.ContrastRatio(Colors.White);
        var contrastWithBlack = background.ContrastRatio(Colors.Black);
        return contrastWithBlack > contrastWithWhite;
    }

    private static Color AdjustColorToTargetRatio(Color foreground, Color background, float targetRatio, ref double offset, bool shouldDarken, float tolerance)
    {
        var ratio = foreground.ContrastRatio(background);
        var finalColor = foreground;
        double? adjust = null;

        while (!IsRatioWithinTolerance(ratio, targetRatio, tolerance) &&
               !Equals(finalColor, Colors.White) &&
               !Equals(finalColor, Colors.Black))
        {
            adjust = CalculateAdjustment(adjust, shouldDarken, ratio < targetRatio);
            offset += adjust ?? 0;

            finalColor = foreground.ShiftLightness(offset);
            ratio = finalColor.ContrastRatio(background);
        }

        return finalColor;
    }

    private static bool IsRatioWithinTolerance(float ratio, float targetRatio, float tolerance)
    {
        return Math.Abs(ratio - targetRatio) <= tolerance;
    }

    private static double? CalculateAdjustment(double? adjust, bool shouldDarken, bool isRatioBelowTarget)
    {
        if (isRatioBelowTarget)
        {
            return shouldDarken
                ? AdjustValue(adjust, positive: false)
                : AdjustValue(adjust, positive: true);
        }

        return shouldDarken
            ? AdjustValue(adjust, positive: true)
            : AdjustValue(adjust, positive: false);
    }

    private static double AdjustValue(double? adjust, bool positive)
    {
        if (adjust.HasValue)
        {
            return positive
                ? Math.Abs(adjust.Value / 2)
                : -Math.Abs(adjust.Value / 2);
        }
        return positive ? 1.0 : -1.0;
    }

    public static Color ContrastingForegroundColor(this Color color)
        => color.IsLightColor() ? Colors.Black : Colors.White;

    private static bool IsLightColor(this Color color)
    {
        var r = RgbSrgb(color.Red);
        var g = RgbSrgb(color.Green);
        var b = RgbSrgb(color.Blue);

        var luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
        return luminance > 0.179;

        static double RgbSrgb(double d)
        {
            // d /= 255.0; disable for Maui
            return d > 0.03928
                ? Math.Pow((d + 0.055) / 1.055, 2.4)
                : d / 12.92;
        }
    }

    public static bool IsDarkColor(this Color color)
        => !IsLightColor(color);

    private static Color ShiftLightness(this Color color, double amount = 1.0f)
    {
        var lab = color.ToLab();
        var shifted = lab with { L = lab.L - LabConstants.Kn * amount };
        return shifted.ToColor();
    }

    private static Color ShiftLightness(this Color color, int amount = 1)
    {
        var lab = color.ToLab();
        var shifted = lab with { L = lab.L - LabConstants.Kn * amount };
        return shifted.ToColor();
    }

    public static Color Darken(this Color color, int amount = 1)
        => color.ShiftLightness(amount);

    public static Color Lighten(this Color color, int amount = 1)
        => color.ShiftLightness(-amount);
}