using System.Resources;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;

namespace MyExpenses.Wpf.Localisations;

public static class LocalizationResources
{
    public static readonly Dictionary<string, ResourceManager> Managers =
        new()
        {
            [nameof(MainWindowResources)] = MainWindowResources.ResourceManager,
            [nameof(AddEditAccountResources)] = AddEditAccountResources.ResourceManager,
        };
}