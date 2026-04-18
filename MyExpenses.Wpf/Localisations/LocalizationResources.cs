using System.Resources;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.DependencyRessources;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.SharedUtils.Resources.Resx.Dialogs;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;

namespace MyExpenses.Wpf.Localisations;

public static class LocalizationResources
{
    public static readonly Dictionary<string, ResourceManager> Managers =
        new()
        {
            [nameof(MainWindowResources)] = MainWindowResources.ResourceManager,
            [nameof(AddEditAccountResources)] = AddEditAccountResources.ResourceManager,
            [nameof(AccountTypeManagementResources)] = AccountTypeManagementResources.ResourceManager,
            [nameof(DialogResources)] = DialogResources.ResourceManager,
            [nameof(DependencyRessources)] = DependencyRessources.ResourceManager,
            [nameof(AccountResources)] = AccountResources.ResourceManager,
        };
}