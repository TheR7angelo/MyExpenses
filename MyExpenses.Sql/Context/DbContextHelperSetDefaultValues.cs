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
        // ReSharper disable HeapView.ObjectAllocation.Evident
        // All the allocations and default values set in this method are mandatory to ensure the database is initialized
        // with consistent and predictable data. These defaults are essential for maintaining expected behavior within
        // the application and avoiding issues related to missing or inconsistent values. The structure ensures robustness
        // and proper data integrity across various system components.
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
        // ReSharper restore HeapView.ObjectAllocation.Evident

        context.TRecursiveFrequencies.AddRange(recursiveFrequencies);
    }

    private static void UpdateDefaultRecursiveFrequency(this DataBaseContext context)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This method ensures that the default values for recursive frequencies are updated in the database.
        // Each value, including its frequency and description, is mapped to an existing record to maintain consistency
        // with the application's predefined default entries. These updates are essential to ensure proper functionality
        // of features dependent on accurate and updated default values.
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident// This method sets the default value for the "TPlace" table. The predefined value
        // (e.g., a place named "Internet") is crucial for ensuring that the system has a consistent
        // default entry that cannot be deleted. This allocation ensures the application behaves as expected,
        // especially in scenarios where a default place is required.
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
        // ReSharper disable HeapView.ObjectAllocation.Evident
        // All the allocations and default values set in this method are mandatory to ensure the database is initialized
        // with consistent and predictable data. These defaults are essential for maintaining expected behavior within
        // the application and avoiding issues related to missing or inconsistent values. The structure ensures robustness
        // and proper data integrity across various system components.
        var paymentModes = new List<TModePayment>
        {
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCard, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankTransfer, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankDirectDebit, CanBeDeleted = false },
            new() { Name = DbContextHelperSetDefaultValuesResources.DefaultTModePaymentNameBankCheck, CanBeDeleted = false }
        };
        // ReSharper restore HeapView.ObjectAllocation.Evident

        context.TModePayments.AddRange(paymentModes);
    }

    private static void UpdateDefaultTModePayment(this DataBaseContext context)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This method updates the default values for the "TModePayment" table. Each predefined mode of payment
        // (e.g., Bank Card, Bank Transfer, etc.) is essential for the system's correct operation. The update ensures
        // that existing records are aligned with the application's predefined defaults, which guarantees consistency
        // and avoids issues related to outdated or missing entries.
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This blacklist of colors contains system-related or non-user-friendly colors (e.g., "Control", "Desktop", "Transparent").
        // These colors are excluded to ensure that only user-visible, aesthetic, and meaningful colors are used in the application.
        // This restriction is crucial for maintaining a clean and consistent user interface experience.
        var blackList = new[]
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This list is initialized to store a collection of "TColor" objects. It serves as the foundation
        // for managing or manipulating color entries within the application. This allocation is necessary
        // to ensure that colors can be organized, processed, or used as required in subsequent operations.
        var colors = new List<TColor>();
        foreach (var knownColor in knownColors)
        {
            var name = EnumHelper<KnownColor>.ToEnumString(knownColor).SplitUpperCaseWord();
            var color = Color.FromKnownColor(knownColor);
            var hex = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // This code adds a new "TColor" object to the list with a specified name and hexadecimal color code.
            // Each color entry is created dynamically and added to the collection to ensure that all required
            // colors are systematically defined and available for use in the application.
            colors.Add(new TColor
            {
                Name = name,
                HexadecimalColorCode = hex
            });
        }

        context.TColors.AddRange(colors);
    }
}