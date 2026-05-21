using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// ViewModel responsible for managing the list of account types, including viewing, creating, editing, and deleting account types.
/// </summary>
public partial class AccountTypeManagementViewModel : ViewModelBase
{
    private readonly IAccountPresentationService _accountService;
    private readonly INavigationWindowService _navigationWindow;
    private readonly IDialogService _dialog;
    private readonly ILogger<AccountTypeManagementViewModel> _logger;

    /// <summary>
    /// Gets the collection of account types displayed in the management view.
    /// </summary>
    public ObservableCollection<AccountTypeViewModel> AccountTypes { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountTypeManagementViewModel"/> class.
    /// </summary>
    /// <param name="accountService">Service providing access to account type data.</param>
    /// <param name="navigationWindow">Service for navigating to account type views.</param>
    /// <param name="dialog">Service for displaying dialog messages.</param>
    /// <param name="logger">Logger for recording errors and events.</param>
    public AccountTypeManagementViewModel(IAccountPresentationService accountService,
        INavigationWindowService navigationWindow,
        IDialogService dialog,
        ILogger<AccountTypeManagementViewModel> logger)
    {
        _accountService = accountService;
        _navigationWindow = navigationWindow;
        _dialog = dialog;
        _logger = logger;

        RegisterMessages();
    }

    /// <summary>
    /// Registers weak reference message handlers for entity changes.
    /// </summary>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int>>(this, OnAccountTypeDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountTypeViewModel>>(this, OnAccountTypeChanged);
    }

    /// <summary>
    /// Handles the deletion notification of an account type by marking it as deleting.
    /// </summary>
    /// <param name="recipient">The message recipient.</param>
    /// <param name="m">The entity changed message containing deletion information.</param>
    private void OnAccountTypeDeleted(object recipient, EntityChangedMessage<int> m)
    {
        if (m.Value.EntityType != DependencyType.AccountType ||
            m.Value.DataAction != DataAction.Delete)
            return;

        var item = AccountTypes.FirstOrDefault(s => s.Id == m.Value.Content);
        item?.IsDeleting = true;
    }

    /// <summary>
    /// Handles updates and additions to account types, applying the changes to the view model.
    /// </summary>
    /// <param name="recipient">The message recipient.</param>
    /// <param name="m">The entity changed message containing account type changes.</param>
    private async void OnAccountTypeChanged(object recipient, EntityChangedMessage<AccountTypeViewModel> m)
    {
        if (m.Value.EntityType != DependencyType.AccountType) return;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(m.Value.Content);
                break;

            case DataAction.Add:
                await ApplyAddAsync(m.Value.Content);
                break;
        }
    }

    /// <summary>
    /// Applies updates to an existing account type in the collection.
    /// </summary>
    /// <param name="vm">The updated account type view model.</param>
    private void ApplyUpdate(AccountTypeViewModel vm)
    {
        var item = AccountTypes.FirstOrDefault(s => s.Id == vm.Id);
        item?.Name = vm.Name ?? string.Empty;
    }

    /// <summary>
    /// Asynchronously adds a new account type to the collection in sorted order.
    /// </summary>
    /// <param name="vm">The new account type view model to add.</param>
    private async Task ApplyAddAsync(AccountTypeViewModel vm)
    {
        AccountTypes.AddAndSort(vm, s => s.Name!);
    }

    /// <summary>
    /// Navigates to the add account type view.
    /// </summary>
    [RelayCommand]
    private void OnAddAccountType()
    {
        _navigationWindow.ShowAddAccountType();
    }

    /// <summary>
    /// Asynchronously navigates to the edit account type view for the specified account type.
    /// </summary>
    /// <param name="item">The account type to view or edit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [RelayCommand]
    private async Task OnViewAccountTypeAsync(AccountTypeViewModel? item, CancellationToken cancellationToken = default)
    {
        try
        {
            if (item is null) return;
            await _navigationWindow.ShowEditAccountTypeAsync(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing account type");
            _dialog.ShowError(AccountResources.MessageBoxViewAccountTypeErrorContent);
        }
    }

    /// <summary>
    /// Removes the specified account type from the collection.
    /// </summary>
    /// <param name="item">The account type to delete.</param>
    [RelayCommand]
    private void OnDelete(AccountTypeViewModel? item)
    {
        if (item is null) return;

        AccountTypes.Remove(item);
    }

    /// <summary>
    /// Asynchronously loads all account types from the service and populates the collection.
    /// </summary>
    [RelayCommand]
    private async Task OnLoadAsync()
    {
        try
        {
            await FillAccountTypes();
        }
        catch (OperationCanceledException)
        {
            // Pass
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading account type");
            _dialog.ShowError(AccountResources.MessageBoxLoadAccountTypeErrorContent);
        }
    }

    /// <summary>
    /// Asynchronously fills the account types collection from the service in sorted order.
    /// </summary>
    private async Task FillAccountTypes()
    {
        AccountTypes.Clear();

        var items = await _accountService.GetAllAccountTypeViewModelAsync();
        AccountTypes.AddRangeAndSort(items, s => s.Name!);
    }
}