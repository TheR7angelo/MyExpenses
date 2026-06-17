using Domain.Models.Validation;
using Mapsui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Resources.Resx.LocationResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Utils;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.Wpf.Pages;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.LocationManagementWindows;

namespace MyExpenses.Wpf.Services;

public class NavigationWindowService(IServiceProvider provider, IDialogService dialogService,
    INominatimPresentationService nominatimPresentationService,
    INavigationService navigationService,
    ILogger<NavigationWindowService> logger,
    INominatimDtoViewModelMapper nominatimDtoViewModelMapper) : INavigationWindowService
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
        var accountActionService = provider.GetRequiredService<IAccountActionService>();
        await accountActionService.ManageAccountTypeAction();
    }

    public async Task ShowEditAccountTypeAsync(AccountTypeViewModel item)
    {
        var accountActionService = provider.GetRequiredService<IAccountActionService>();
        await accountActionService.ManageAccountTypeAction(item);
    }

    public void ShowManageCategoryType(CategoryTypeViewModel? categoryTypeViewModel)
    {
        var window = provider.GetRequiredService<AddEditCategoryTypeWindow>();
        if (categoryTypeViewModel is not null) window.LoadCategoryTypeViewModel(categoryTypeViewModel);
        window.ShowDialog();
    }

    public void ShowLocationManagementWindow(PlaceViewModel? placeViewModel, bool isEdit)
    {
        placeViewModel ??= new PlaceViewModel();

        var window = provider.GetRequiredService<AddEditLocationWindow>();
        window.LoadPlaceViewModel(placeViewModel, isEdit);
        window.ShowDialog();
    }

    public async Task ShowLocationManagementWindow(MPoint point, CancellationToken cancellationToken = default)
    {
        var results = await nominatimPresentationService.SearchAsync(point.Y, point.X, cancellationToken);
        var placeViewModel = ManageLocationWindowAction(results);

        if (placeViewModel is null) return;
        ShowLocationManagementWindow(placeViewModel, false);
    }

    public PlaceViewModel? ManageLocationWindowAction(Result<IEnumerable<NominatimSearchResultViewModel>> results)
    {
        if (!results.IsSuccess)
        {
            logger.LogError("Error while searching locations: {Message}", results.InternalMessage);
            var message = string.Format(LocationResources.NominatimServiceSearchErrorContent, results.ErrorCode);
            dialogService.ShowMessageBox(LocationResources.NominatimServiceSearchErrorTitle,
                message, MessageBoxButton.Ok, MsgBoxImage.Error);
            return null;
        }

        if (!results.Value!.Any())
        {
            dialogService.ShowMessageBox(LocationResources.NominatimServiceSearchErrorAnyValueTitle,
                LocationResources.NominatimServiceSearchErrorAnyValueContent, MessageBoxButton.Ok, MsgBoxImage.Information);
            return null;
        }

        var values = results.Value!;

        var selectedNominatim = results.Value!.Count() > 1
            ? ShowLocationManagementWindow(values)
            : values.First();
        if (selectedNominatim is null) return null;

        var placeViewModel = nominatimDtoViewModelMapper.MapToPlaceViewModel(selectedNominatim);
        placeViewModel.AcceptChanges();

        return placeViewModel;
    }

    public void ManageExpense(HistoryViewModel? historyViewModel)
        => navigationService.Navigate(nameof(RecordExpensePage), historyViewModel);

    public NominatimSearchResultViewModel? ShowLocationManagementWindow(IEnumerable<NominatimSearchResultViewModel> nominatimSearchResultViewModels)
    {
        var window = provider.GetRequiredService<NominatimSearchWindow>();
        window.LoadNominatimSearchResults(nominatimSearchResultViewModels);
        window.ShowDialog();

        return window.DialogResult is not true
            ? null
            : window.CurrentSearchResult;
    }

    public void ShowColorManagementWindow(ColorViewModel? color)
    {
        var window = provider.GetRequiredService<AddEditColorWindow>();
        if (color is not null) window.LoadColorViewModel(color);
        window.ShowDialog();
    }

    public void ShowRecurringExpenseWindow()
    {
        var window = provider.GetRequiredService<RecurrentAddWindow>();
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
