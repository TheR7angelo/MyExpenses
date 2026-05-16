using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;

/// <summary>
/// Represents a window used for adding or editing category types in the application.
/// This class provides the ability to manage category type details within the context of a category type management workflow.
/// </summary>
public partial class AddEditCategoryTypeWindow : IClosable
{
    /// <summary>
    /// Gets the <see cref="CategoryTypeManagementViewModel"/> instance used as the data context
    /// for the current window. This serves as the primary view model for managing category types,
    /// providing properties, commands, and bound data required for managing and interacting
    /// with category type data within the view.
    /// </summary>
    private CategoryTypeManagementViewModel ViewModel => (CategoryTypeManagementViewModel)DataContext;

    /// <summary>
    /// Represents a window for adding or editing a category type within the context of category type management.
    /// Provides functionality to load, modify, and manage category type details and related operations.
    /// </summary>
    public AddEditCategoryTypeWindow(CategoryTypeManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadAllColorCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Loads the specified <see cref="CategoryTypeViewModel"/> into the current view model
    /// for handling operations related to category type management, such as editing or displaying details.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// The instance of <see cref="CategoryTypeViewModel"/> to load into the current window's view model.
    /// </param>
    public void LoadCategoryTypeViewModel(CategoryTypeViewModel categoryTypeViewModel)
        => ViewModel.LoadCategoryTypeViewModel(categoryTypeViewModel);
}