using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Services;

public class NavigationServices(IServiceProvider provider) : INavigationServices
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
