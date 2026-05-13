using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public class CategoryTypeManagementViewModel : ViewModelBase
{
    private readonly IExpensePresentationService _expensePresentationService;

    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    public IAsyncRelayCommand LoadCommand { get; }

    public CategoryTypeManagementViewModel(IExpensePresentationService expensePresentationService)
    {
        _expensePresentationService = expensePresentationService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
    }

    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var categoryType = await _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        CategoryTypeViewModels.AddRangeAndSort(categoryType, vm => vm.Name!);
    }
}