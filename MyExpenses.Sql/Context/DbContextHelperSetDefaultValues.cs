using System.Drawing;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Resources.Resx.DbContextHelperSetDefaultValues;
using MyExpenses.Sql.Utils.Regex;

namespace MyExpenses.Sql.Context;

public static class DbContextHelperSetDefaultValues
{
    public static bool SetAllDefaultValues(this DataBaseContext context)
    {
        try
        {
            context.SetDefaultTColor();
            context.SetDefaultTModePayment();
            context.SetDefaultTPlace();
            context.SetDefaultRecursiveFrequency();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    //TODO work
    private static void SetDefaultRecursiveFrequency(this DataBaseContext context)
    {

    }

    private static void SetDefaultTPlace(this DataBaseContext context)
    {
        var place = new TPlace { Name = DbContextHelperSetDefaultValuesResources.DefautTPlaceNameInternet, CanBeDeleted = false };
        context.TPlaces.Add(place);
    }

    public static void UpdateDefaultTPlace(this DataBaseContext context)
    {
        var oldDefaultTPlace = context.TPlaces.First(s => s.Id.Equals(1));
        oldDefaultTPlace.Name = DbContextHelperSetDefaultValuesResources.DefautTPlaceNameInternet;
    }

    private static void SetDefaultTModePayment(this DataBaseContext context)
    {
        var paymentMode = new List<TModePayment>
        {
            new() { Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankCard, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankTransfer, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankDirectDebit, CanBeDeleted = false }
        };
        context.TModePayments.AddRange(paymentMode);
    }

    public static void UpdateDefaultTModePayment(this DataBaseContext context)
    {
        var oldDefaultPaymentModeBankCard = context.TModePayments.First(s => s.Id.Equals(1));
        var oldDefaultPaymentModeBankTransfer = context.TModePayments.First(s => s.Id.Equals(2));
        var oldDefaultPaymentModeBankDirectDebit = context.TModePayments.First(s => s.Id.Equals(3));

        oldDefaultPaymentModeBankCard.Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankCard;
        oldDefaultPaymentModeBankTransfer.Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankTransfer;
        oldDefaultPaymentModeBankDirectDebit.Name = DbContextHelperSetDefaultValuesResources.DefautTModePaymentNameBankDirectDebit;
    }

    private static void SetDefaultTColor(this DataBaseContext context)
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
    }
}