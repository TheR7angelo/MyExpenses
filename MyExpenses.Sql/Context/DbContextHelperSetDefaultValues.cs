using System.Drawing;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Utils;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.Sql.Resources.Resx.DbContextHelperSetDefaultValues;

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

    public static bool UpdateAllDefaultValues(this DataBaseContext context)
    {
        try
        {
            context.UpdateDefaultTModePayment();
            context.UpdateDefaultTPlace();
            context.UpdateDefaultRecursiveFrequency();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static void SetDefaultRecursiveFrequency(this DataBaseContext context)
    {
        var recursiveFrequencies = new List<TRecursiveFrequency>
        {
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyDaily,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyDailyDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyWeekly,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyWeeklyDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyMonthly,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyMonthlyDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyBimonthly,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyBimonthlyDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuarterly,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuarterlyDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuadrimestriel,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuadrimestrielDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencySemiAnnual,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencySemiAnnualDefinition
            },
            new()
            {
                Frequency = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyYearly,
                Description = DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyYearlyDefinition
            }
        };

        context.TRecursiveFrequencies.AddRange(recursiveFrequencies);
    }

    private static void UpdateDefaultRecursiveFrequency(this DataBaseContext context)
    {
        var oldDefaultTRecursiveFrequency = new List<(string Frequency, string Description)>
        {
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyDaily,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyDailyDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyWeekly,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyWeeklyDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyMonthly,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyMonthlyDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyBimonthly,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyBimonthlyDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuarterly,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuarterlyDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuadrimestriel,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyQuadrimestrielDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencySemiAnnual,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencySemiAnnualDefinition),
            (DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyYearly,
                DbContextHelperSetDefaultValuesResources.DefaultTRecursiveFrequencyYearlyDefinition)
        };

        var records = context.TRecursiveFrequencies.Take(oldDefaultTRecursiveFrequency.Count).AsEnumerable();
        foreach (var record in records)
        {
            record.Frequency = oldDefaultTRecursiveFrequency[record.Id - 1].Frequency;
            record.Description = oldDefaultTRecursiveFrequency[record.Id - 1].Description;
        }
    }

    private static void SetDefaultTPlace(this DataBaseContext context)
    {
        var place = new TPlace { Name = DbContextHelperSetDefaultValuesResources.DefaultTPlaceNameInternet, CanBeDeleted = false };
        context.TPlaces.Add(place);
    }

    private static void UpdateDefaultTPlace(this DataBaseContext context)
    {
        var oldDefaultTPlace = context.TPlaces.First(s => s.Id.Equals(1));
        oldDefaultTPlace.Name = DbContextHelperSetDefaultValuesResources.DefaultTPlaceNameInternet;
    }

    private static void SetDefaultTModePayment(this DataBaseContext context)
    {
        var paymentModes = new List<TModePayment>
        {
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCard, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankTransfer, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankDirectDebit, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCheck, CanBeDeleted = false }
        };
        context.TModePayments.AddRange(paymentModes);
    }

    private static void UpdateDefaultTModePayment(this DataBaseContext context)
    {
        var oldDefaultTModePayments = new List<string>
        {
            DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCard,
            DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankTransfer,
            DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankDirectDebit,
            DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCheck
        };

        var records = context.TModePayments.Take(oldDefaultTModePayments.Count).AsEnumerable();
        foreach (var record in records)
        {
            record.Name = oldDefaultTModePayments[record.Id - 1];
        }
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
            .OrderBy(s => s).AsEnumerable();

        var colors = new List<TColor>();
        foreach (var knownColor in knownColors)
        {
            var name = EnumHelper<KnownColor>.ToEnumString(knownColor).SplitUpperCaseWord();
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