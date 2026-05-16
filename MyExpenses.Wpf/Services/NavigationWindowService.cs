using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;

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

    public void ShowEditAccount(AccountViewModel? vm)
    {
        var window = provider.GetRequiredService<AddEditAccountWindow>();
        window.Load(vm);
        window.ShowDialog();
    }

    public async Task ShowAddAccountType()
    {
        var accountActionService = App.ServiceProvider.GetRequiredService<IAccountActionService>();
        await accountActionService.ManageAccountTypeAction();
    }

    public async Task ShowEditAccountTypeAsync(AccountTypeViewModel item)
    {
        var accountActionService = App.ServiceProvider.GetRequiredService<IAccountActionService>();
        await accountActionService.ManageAccountTypeAction(item);
    }

    public void ShowManageCategoryType(CategoryTypeViewModel? categoryTypeViewModel)
    {
        var window = provider.GetRequiredService<AddEditCategoryTypeWindow>();
        if (categoryTypeViewModel is not null) window.LoadCategoryTypeViewModel(categoryTypeViewModel);
        window.ShowDialog();
    }
}
