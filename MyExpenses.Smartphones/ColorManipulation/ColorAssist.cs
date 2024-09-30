using MyExpenses.Models.Ui.Maui;

namespace MyExpenses.Smartphones.ColorManipulation;

public static class ColorAssist
{
    public static float RelativeLuminance(this Color color)
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
    public static float ContrastRatio(this Color color, Color color2)
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
        => EnsureContrastRatio(foreground, background, targetRatio, out _, tolerance);

    /// <summary>
    /// Adjust the foreground color to have an acceptable contrast ratio.
    /// </summary>
    /// <param name="foreground">The foreground color</param>
    /// <param name="background">The background color</param>
    /// <param name="targetRatio">The target contrast ratio</param>
    /// <param name="offset">The offset that was applied</param>
    /// <param name="tolerance">The tolerance to the contrast ratio needs to be within</param>
    /// <returns>The updated foreground color with the target contrast ratio with the background</returns>
    public static Color EnsureContrastRatio(this Color foreground, Color background, float targetRatio, out double offset, float tolerance = 0.1f)
    {
        offset = 0.0f;
        var ratio = foreground.ContrastRatio(background);
        if (ratio > targetRatio) return foreground;

        var contrastWithWhite = background.ContrastRatio(Colors.White);
        var contrastWithBlack = background.ContrastRatio(Colors.Black);

        var shouldDarken = contrastWithBlack > contrastWithWhite;

        //Lighten is negative
        var finalColor = foreground;
        double? adjust = null;

        while ((ratio < targetRatio - tolerance || ratio > targetRatio + tolerance) &&
               finalColor != Colors.White &&
               finalColor != Colors.Black)
        {
            if (ratio - targetRatio < 0.0)
            {
                //Move offset of foreground further away from background
                if (shouldDarken)
                {
                    if (adjust < 0)
                    {
                        adjust /= -2;
                    }
                    else
                    {
                        adjust ??= 1.0f;
                    }
                }
                else
                {
                    if (adjust > 0)
                    {
                        adjust /= -2;
                    }
                    else
                    {
                        adjust ??= -1.0f;
                    }
                }
            }
            else
            {
                //Move offset of foreground closer to background
                if (shouldDarken)
                {
                    if (adjust > 0)
                    {
                        adjust /= -2;
                    }
                    else
                    {
                        adjust ??= -1.0f;
                    }
                }
                else
                {
                    if (adjust < 0)
                    {
                        adjust /= -2;
                    }
                    else
                    {
                        adjust ??= 1.0f;
                    }
                }
            }

            offset += adjust.Value;

            finalColor = foreground.ShiftLightness(offset);

            ratio = finalColor.ContrastRatio(background);
        }
        return finalColor;
    }

    public static Color ContrastingForegroundColor(this Color color)
        => color.IsLightColor() ? Colors.Black : Colors.White;

    public static bool IsLightColor(this Color color)
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

    public static Color ShiftLightness(this Color color, double amount = 1.0f)
    {
        var lab = color.ToLab();
        var shifted = lab with { L = lab.L - LabConstants.Kn * amount };
        return shifted.ToColor();
    }

    public static Color ShiftLightness(this Color color, int amount = 1)
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