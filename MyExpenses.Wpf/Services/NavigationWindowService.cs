using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Services;

public class NavigationWindowService(IServiceProvider provider) : INavigationWindowService
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

    public async Task ShowAddAccountType()
    {
        var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
        await accountActionService.ManageAccountTypeAction();
    }

    public async Task ShowEditAccountTypeAsync(AccountTypeViewModel item)
    {
        var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
        await accountActionService.ManageAccountTypeAction(item);
    }
}
