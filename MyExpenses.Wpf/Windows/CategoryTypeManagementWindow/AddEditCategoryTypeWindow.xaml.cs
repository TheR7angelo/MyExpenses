using System.Windows;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;

public partial class AddEditCategoryTypeWindow : IClosable
{
    private CategoryTypeManagementViewModel ViewModel => (CategoryTypeManagementViewModel)DataContext;

    public AddEditCategoryTypeWindow(CategoryTypeManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadAllColorCommand.ExecuteAsync(null);
    }

    public void LoadCategoryTypeViewModel(CategoryTypeViewModel categoryTypeViewModel)
        => ViewModel.LoadCategoryTypeViewModel(categoryTypeViewModel);

    private void ButtonAddColor_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // if (CategoryType.ColorFk is not null) EditColor();
        // else CreateNewColor();
    }
}