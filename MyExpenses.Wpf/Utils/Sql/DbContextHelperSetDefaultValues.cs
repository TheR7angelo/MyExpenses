using System.Drawing;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Regex;

namespace MyExpenses.Wpf.Utils.Sql;

public static class DbContextHelperSetDefaultValues
{
    public static bool SetDefaultValues(this DataBaseContext context)
    {
        SetDefaultTColor(context);

        return true;
    }

    private static bool SetDefaultTColor(DataBaseContext context)
    {
        var blackList = new List<KnownColor>
        {
            KnownColor.Control, KnownColor.Desktop, KnownColor.Highlight, KnownColor.Info, KnownColor.Menu,
            KnownColor.Transparent, KnownColor.Window, KnownColor.ActiveBorder, KnownColor.ActiveCaption,
            KnownColor.AppWorkspace, KnownColor.ButtonFace, KnownColor.ButtonHighlight, KnownColor.ButtonShadow,
            KnownColor.ControlDark, KnownColor.ControlLight, KnownColor.ControlText, KnownColor.GrayText,
            KnownColor.HighlightText, KnownColor.HotTrack, KnownColor.InactiveBorder, KnownColor.InactiveCaption,
            KnownColor.InactiveCaptionText, KnownColor.InfoText, KnownColor.MenuBar, KnownColor.MenuHighlight,
            KnownColor.MenuText, KnownColor.ScrollBar, KnownColor.WindowFrame, KnownColor.WindowText,
            KnownColor.ActiveCaptionText, KnownColor.ControlDarkDark, KnownColor.ControlLightLight,
            KnownColor.GradientActiveCaption, KnownColor.GradientInactiveCaption
        };
        var knownColors = Enum.GetValues<KnownColor>()
            .Where(s => !blackList.Contains(s))
            .OrderBy(s => s.ToString())
            .ToList();

        var colors = new List<TColor>();
        foreach (var knownColor in knownColors)
        {
            var name = knownColor.ToString().SplitUpperCaseWord();
            var color = Color.FromKnownColor(knownColor);
            var hex = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

            colors.Add(new TColor
            {
                Name = name,
                HexadecimalColorCode = hex
            });
        }

        context.TColors.AddRange(colors);

        return true;
    }
}