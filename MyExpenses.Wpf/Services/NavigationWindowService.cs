using Mapsui;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Utils;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.LocationManagementWindows;

namespace MyExpenses.Wpf.Services;

public class NavigationWindowService(IServiceProvider provider, IDialogService dialogService) : INavigationWindowService
{
    public async Task ShowManageAccount(TotalByAccountViewModel? item)
    {
        var window = provider.GetRequiredService<AddEditAccountWindow>();
        if (item is not null) await window.LoadAsync(item);
        window.ShowDialog();
    }

    public void ShowManageAccount(AccountViewModel? vm)
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

    public void ShowLocationManagementWindow(PlaceViewModel? placeViewModel)
    {
        var window = provider.GetRequiredService<AddEditLocationWindow>();
        if (placeViewModel is not null) window.LoadPlaceViewModel(placeViewModel);
        window.ShowDialog();
    }

    public async Task ShowLocationManagementWindow(MPoint point, CancellationToken cancellationToken = default)
    {
        var nominatimService = provider.GetRequiredService<INominatimService>();
        var results = await nominatimService.SearchAsync(point.Y, point.X, cancellationToken);
        if (!results.IsSuccess)
        {
            dialogService.ShowMessageBox("Error", results.InternalMessage!, MessageBoxButton.Ok, MsgBoxImage.Error);
        }
        else if (!results.Value!.Any())
        {
            dialogService.ShowMessageBox("Info", "No location found!", MessageBoxButton.Ok, MsgBoxImage.Information);
        }
        else if (results.Value!.Count() is 1)
        {
            // TODO map nominatim to placeviewmodel
        }
        else
        {
            // TODO show multiple locations choice
        }
    }

    public void ShowColorManagementWindow(ColorViewModel? color)
    {
        var window = App.ServiceProvider.GetRequiredService<AddEditColorWindow>();
        if (color is not null) window.LoadColorViewModel(color);
        window.ShowDialog();
    }

    public void OpenGithubPage()
    {
        const string url = "https://github.com/TheR7angelo/MyExpenses";
        url.StartProcess();
    }

    public void OpenUri(string uri)
    {
        uri.StartProcess();
    }
}
