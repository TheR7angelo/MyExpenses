using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Services;

public class NavigationService(IServiceProvider provider) : INavigationService
{
    public void ShowAddAccount()
    {
        var window = provider.GetRequiredService<AddEditAccountWindow>();
        window.ShowDialog();
    }

    public async Task ShowEditAccountAsync(TotalByAccountViewModel vm)
    {
        var window = provider.GetRequiredService<AddEditAccountWindow>();
        await window.LoadAsync(vm);
        window.ShowDialog();
    }
}
