using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Represents the view model responsible for managing a collection of currencies
/// for the application. Provides functionalities such as loading the list of currencies,
/// removing specific currencies from the list, and managing individual currency records.
/// </summary>
public partial class CurrencyManagementViewModel : ViewModelBase
{
    /// <summary>
    /// Handles the retrieval and management of data related to accounts, account types,
    /// and currencies for presentation purposes. This service is used to fetch
    /// relevant information, such as a collection of currency view models, and to
    /// perform high-level operations tied to account-related presentation logic.
    /// </summary>
    private readonly IAccountPresentationService _accountPresentationService;

    /// <summary>
    /// Provides functionality to handle actions related to accounts and currencies
    /// across the application. This service enables the management, creation,
    /// updating, and deletion of account types, accounts, and currencies,
    /// ensuring a unified approach for triggering and executing related operations.
    /// </summary>
    private readonly IAccountActionService _accountActionService;

    /// <summary>
    /// Provides functionality to map between DTO (Data Transfer Objects) and ViewModel representations
    /// specific to accounts and related entities. This mapper facilitates the conversion, merging,
    /// and cloning of object data between layers, ensuring consistent and robust data transformations
    /// within the application.
    /// </summary>
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    /// <summary>
    /// Gets the collection of currency view models that represent individual currency entries in the application.
    /// This property provides an observable collection of <see cref="CurrencyViewModel"/>, serving as the data source
    /// for displaying and managing currencies within the user interface. Modifications to the collection, such as adding,
    /// removing, or updating currencies, will automatically reflect in the bound UI elements, ensuring a dynamic and
    /// synchronized experience.
    /// </summary>
    public ObservableCollection<CurrencyViewModel> CurrencyViewModels { get; } = [];

    /// <summary>
    /// Represents the view model responsible for managing a collection of currencies
    /// and their respective operations within the application. It provides commands
    /// for managing, removing, and loading currencies, while using services for
    /// data manipulation and integration.
    /// </summary>
    public CurrencyManagementViewModel(IAccountPresentationService accountPresentationService,
        IAccountActionService accountActionService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper)
    {
        _accountPresentationService = accountPresentationService;
        _accountActionService = accountActionService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;

        RegisterMessages();
    }

    /// <summary>
    /// Invokes the appropriate action to manage the specified currency. This may include updates,
    /// modifications, or other relevant operations on the currency.
    /// </summary>
    /// <param name="item">The currency view model representing the currency to be managed. Can be null.</param>
    [RelayCommand]
    private void OnManageCurrency(CurrencyViewModel? item)
        => _accountActionService.ManageCurrencyAction(item);

    /// <summary>
    /// Registers the necessary message handlers for responding to entity change notifications
    /// relevant to currency data in the application. This includes handlers for currency addition,
    /// modification, and deletion messages.
    /// </summary>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnDeleteCurrency);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CurrencyViewModel>>(this, OnCurrencyChanged);
    }

    /// <summary>
    /// Handles changes to currency-related data and updates the collection of currency view models
    /// based on the specified data action (add or update).
    /// </summary>
    /// <param name="recipient">The recipient object that listens to the message.</param>
    /// <param name="message">The message containing details about the changed currency entity,
    /// including its type, the action performed, and the associated data.</param>
    private void OnCurrencyChanged(object recipient, EntityChangedMessage<CurrencyViewModel> message)
    {
        if (message.Value.EntityType is not DependencyType.Currency) return;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (message.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(message.Value.Content);
                break;

            case DataAction.Add:
                ApplyAddAsync(message.Value.Content);
                break;
        }
    }

    /// <summary>
    /// Adds a new currency to the collection and sorts the collection based on the currency's symbolic representation.
    /// The new currency is inserted in a position that maintains the sort order.
    /// </summary>
    /// <param name="item">The currency view model to be added to the collection. The symbol of the currency is used as the sorting key.</param>
    private void ApplyAddAsync(CurrencyViewModel item)
        => CurrencyViewModels.AddAndSort(item, vm => vm.Symbol!);

    /// <summary>
    /// Updates an existing currency in the collection by merging new data from the provided view model.
    /// If a matching currency is found based on the ID, its properties are updated using the specified mapper.
    /// </summary>
    /// <param name="vm">The currency view model containing updated data to apply to the existing item in the collection.</param>
    private void ApplyUpdate(CurrencyViewModel vm)
    {
        var item = CurrencyViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _accountDtoViewModelMapper.Merge(vm, item);
    }

    /// <summary>
    /// Handles the deletion of currencies based on the provided message.
    /// Updates the IsDeleting property for any currencies whose IDs are specified in the message content.
    /// </summary>
    /// <param name="recipient">The recipient object receiving the message, typically the current view model instance.</param>
    /// <param name="message">The message containing information about the deletion action, including the affected entity type and IDs.</param>
    private void OnDeleteCurrency(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.DataAction is not DataAction.Delete || message.Value.EntityType is not DependencyType.Currency) return;

        foreach (var item in CurrencyViewModels.Where(s => message.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    /// <summary>
    /// Removes the specified currency from the collection of currency view models.
    /// </summary>
    /// <param name="item">The currency view model to be removed. If null, the method does nothing.</param>
    [RelayCommand]
    private void OnRemove(CurrencyViewModel? item)
    {
        if (item is null) return;

        CurrencyViewModels.Remove(item);
    }

    /// <summary>
    /// Asynchronously loads and initializes the currency data, sorting it by the currency symbol.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        var currencies = await _accountPresentationService.GetAllCurrencyViewModelAsync(cancellationToken);
        CurrencyViewModels.AddRangeAndSort(currencies, s => s.Symbol!);
    }
}