using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class ModePaymentManagementViewModel : ViewModelBase
{
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly IExpenseActionService _expenseActionService;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;

    public ObservableCollection<ModePaymentViewModel> ModePaymentViewModels { get; } = [];

    public ModePaymentManagementViewModel(IExpensePresentationService expensePresentationService,
        IExpenseActionService expenseActionService, IExpenseDtoViewModelMapper expenseDtoViewModelMapper)
    {
        _expensePresentationService = expensePresentationService;
        _expenseActionService = expenseActionService;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;

        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnDeleteModePayment);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<ModePaymentViewModel>>(this, OnModePaymentChanged);
    }

    [RelayCommand]
    private void OnManageModePayement(ModePaymentViewModel? item)
    {
        if (item is null || item.CanBeDeleted) _expenseActionService.ManageModePaymentAction(item);
        else
        {
            // TODO send message cant edit or delete
        }
    }

    private void OnModePaymentChanged(object recipient, EntityChangedMessage<ModePaymentViewModel> message)
    {
        if (message.Value.EntityType is not DependencyType.ModePayment) return;

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


    private void ApplyAddAsync(ModePaymentViewModel item)
        => ModePaymentViewModels.AddAndSort(item, vm => vm.Name!);


    private void ApplyUpdate(ModePaymentViewModel vm)
    {
        var item = ModePaymentViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _expenseDtoViewModelMapper.Merge(vm, item);
    }

    private void OnDeleteModePayment(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.DataAction is not DataAction.Delete || message.Value.EntityType is not DependencyType.ModePayment) return;

        foreach (var item in ModePaymentViewModels.Where(s => message.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    [RelayCommand]
    private void OnRemove(ModePaymentViewModel? item)
    {
        if (item is null) return;

        ModePaymentViewModels.Remove(item);
    }

    /// <summary>
    /// Asynchronously loads and initializes the currency data, sorting it by the currency symbol.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        var modePaymentViewModels = await _expensePresentationService.GetAllModePaymentViewModelAsync(cancellationToken);
        ModePaymentViewModels.AddRangeAndSort(modePaymentViewModels, s => s.Name!);
    }
}